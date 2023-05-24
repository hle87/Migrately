import React, { useState, useEffect } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import migratelyLogo from "../../assets/images/migrately/migrately-logo.png";
import FileUpload from "components/fileUpload/FileUpload";
import PropTypes from "prop-types";
import "./userstyles.css";
import debug from "sabio-debug";
import { Card, Col, Row, Image, Button } from "react-bootstrap";
import { Formik, Form, Field, ErrorMessage } from "formik";
import * as validation from "../../schemas/userSchema";
import userService from "services/userService";
import toastr from "toastr";
import Swal from "sweetalert2";
import useToggle from "@react-hook/toggle";
import InputGroup from "react-bootstrap/InputGroup";

const _logger = debug.extend("User Change Form");
const _logger2FA = _logger.extend("2FA");

const UserSettingsChange = (props) => {
  const currentUser = props.currentUser;
  _logger(currentUser);
  const navigate = useNavigate();
  const { id } = useParams();
  const [userExistingData] = useState({
    firstName: "",
    lastName: "",
    avatarUrl: "",
  });

  const [is2FAEnabled, setIs2FAEnabled] = useToggle(false);
  const [user2FAData, setUser2FAData] = useState({
    phoneNumber: "",
    isActive: true,
    is2FAEnabled: true,
    //userId: "",
  });
  const [phoneDisplayValue, setPhoneDisplayValue] = useState({
    phoneNumber: "",
  });

  const [mergedUserExistingData, setMergedUserExistingData] = useState({
    ...userExistingData,
    phoneNumber: user2FAData.phoneNumber,
  });

  _logger2FA("mergedUserExistingData", mergedUserExistingData);

  const handleToggle = () => {
    setIs2FAEnabled(!is2FAEnabled);
  };

  const onPhoneNumChange = (e) => {
    const { value } = e.target;
    setPhoneDisplayValue((prevState) => {
      const formattedValue = value
        .replace(/[^0-9]+/g, "")
        .replace(/(\d{3})(\d{3})(\d{4})/, "($1) $2-$3");
      const phoneDisplayValue = { ...prevState, phoneNumber: formattedValue };
      return phoneDisplayValue;
    });
    setUser2FAData((prevState) => {
      const noCharactersValue = value.replace(/[^0-9]+/g, "");
      const user2FAData = { ...prevState, phoneNumber: noCharactersValue };
      return user2FAData;
    });
  };

  const onAddEditButtonSubmit = (e) => {
    e.preventDefault();
    _logger2FA("values", user2FAData);
    if (user2FAData.phoneNumber === /\d/) {
      // const handleAddPhoneNumber = (e) => {
      //   e.preventDefault();
      //   _logger2FA("values", user2FAData);
      setUser2FAData((prevState) => {
        const reformatValue = prevState.phoneNumber.replace(/[^0-9]+/g, "");
        const user2FAData = {
          ...prevState,
          phoneNumber: reformatValue,
          userId: id,
        };
        return user2FAData;
      });
      userService // try to revert number here
        .addPhoneFor2FA(user2FAData)
        .then(onAddPhoneSuccess)
        .catch(onAddPhoneError);
    } else {
      // const onEditPhoneNumber = (e) => {
      //   e.preventDefault();
      //   _logger2FA("values", user2FAData);
      setUser2FAData((prevState) => {
        const reformatValue = prevState.phoneNumber.replace(/[^0-9]+/g, "");
        const user2FAData = {
          ...prevState,
          phoneNumber: reformatValue,
          userId: id,
        };
        return user2FAData;
      });
      userService
        .editPhoneFor2FA(user2FAData)
        .then(onEditPhoneSuccess)
        .catch(onEditPhoneError);
    }
  };

  const onAddPhoneSuccess = (response) => {
    toastr.success("Success! Phone number added!", "Success");
    _logger2FA("Phone number added", response);
  };

  const onAddPhoneError = (err) => {
    toastr.error("Something's wrong! Error adding phone number", "Error");
    _logger("Phone number error", err);
  };

  const onEditPhoneSuccess = (response) => {
    toastr.success("Success! Phone number edited!", "Success");
    _logger2FA("Phone number edited", response);
  };

  const onEditPhoneError = (err) => {
    toastr.error("Something's wrong! Error editing phone number", "Error");
    _logger2FA("Phone number error", err);
  };

  const getUser = (userId) => {
    userService.getCurrentUser(userId).then(getUserSuccess).catch(getUserError);
  };

  const getUserById = (id) => {
    userService
      .getUserById(id)
      .then(getUserByIdSuccess)
      .catch(getUserByIdError);
  };

  // const getIdFor2FA = (id) => {
  //   userService
  //     .getIdFor2FA(id)
  //     .then(getIdFor2FASuccess)
  //     .catch(getIdFor2FAError);
  // };
  // _logger2FA("user2FAData", getIdFor2FA);

  // const getIdFor2FASuccess = (response) => {
  //   _logger2FA("2FA Response--->", response);
  //   const user2FAData = { ...response.data.item };
  //   return user2FAData;
  // };

  // const getIdFor2FAError = (response) => {
  //   _logger2FA("2FA Error", response);
  // };

  useEffect(() => {
    getUser();
  }, []);

  const getUserByIdSuccess = (userResponse) => {
    toastr.success("Success! User Data Retrieved!", "Success");
    setMergedUserExistingData((prevState) => {
      _logger("userData Response--->", userResponse);
      const userData = { ...prevState, ...userResponse.data.item };
      return userData;
    });
  };

  const getUserByIdError = (response) => {
    toastr.error("Something's wrong! Error retrieving User Id", "Error");
    _logger("User Data Error", response);
  };

  const getUserSuccess = (userResponse) => {
    toastr.success("Success! User Data Retrieved!", "Success");
    _logger("userData Response--->", userResponse);
    const userId = userResponse.data.item.id;
    getUserById(userId);
  };

  const getUserError = (response) => {
    toastr.error("Something's wrong! Error retrieving User data", "Error");
    _logger("User Data Error", response);
  };

  const onPasswordChange = (e) => {
    e.preventDefault();
    Swal.fire({
      title: "Would you like to change your password?",
      icon: "question",
      showCancelButton: true,
      confirmButtonText: "Yes",
    }).then((result) => {
      if (result["isConfirmed"]) {
        toastr.info("On the move!", "Redirecting", "Info");
        navigate("/resetpassword");
      }
    });
  };

  const handleSubmit = (values) => {
    Swal.fire({
      title: "Update your information?",
      icon: "question",
      showCancelButton: true,
      confirmButtonText: "Yes",
    }).then((result) => {
      if (result["isConfirmed"]) {
        userService
          .updateUserData(id, values)
          .then(onUpdateSuccess)
          .catch(onUpdateError);
      } else if (result["isDenied"]) {
        Swal.fire("Changes are not saved", "", "info");
      }
    });
  };

  const onUpdateSuccess = (response) => {
    setMergedUserExistingData((prevstate) => {
      const prevUserData = { ...prevstate };
      _logger("update response--->", response);
      return prevUserData;
    });
    Swal.fire("Awesome!", "You updated your info!!", "success");
  };

  const onUpdateError = (e, response) => {
    e.preventDefault();
    toastr.error("Something's wrong! Can't Update", "Error");
    _logger(response);
  };

  const uploadFile = (file, setFieldValue) => {
    setFieldValue("imageUrl", file[0].url);
  };

  return (
    <React.Fragment>
      <div className="row py-6 px-12 migrately-theme-background">
        <Row className="align-items-center justify-content-center g-0 min-vh-100">
          <Col lg={7} md={5} className="py-8 py-xl-0">
            <Card>
              <Card.Body className="p-6">
                <div className="mb-4">
                  <div className="user-align-center">
                    <Link to="/">
                      <Image
                        src={migratelyLogo}
                        className="mb-4 user-align-center"
                        alt="Migrately Logo"
                      />
                    </Link>
                  </div>
                  <h2 className="fw-bold user-align-center">
                    User Settings Change Form
                  </h2>
                </div>

                <Formik
                  enableReinitialize={true}
                  initialValues={mergedUserExistingData}
                  onSubmit={(onPasswordChange, handleSubmit)}
                  validationSchema={validation.validationChangeUserSettings}
                >
                  {({ setFieldValue }) => (
                    <Form>
                      <Row>
                        <Col lg={6} md={12} className="mb-3">
                          <ErrorMessage
                            name="firstName"
                            className="user-form-error"
                            component="div"
                          />
                          <label
                            htmlFor="firstName"
                            className="user-form-label-text"
                          >
                            First Name
                          </label>
                          <Field
                            type="text"
                            className="form-control"
                            name="firstName"
                          />
                        </Col>
                        <Col lg={6} md={12} className="mb-3">
                          <label
                            htmlFor="lastName"
                            className="user-form-label-text"
                          >
                            Last Name
                          </label>
                          <Field
                            type="text"
                            className="form-control"
                            name="lastName"
                          />
                        </Col>
                        <div className="mb-3">
                          <ErrorMessage
                            name="avatarUrl"
                            className="user-form-error"
                            component="div"
                          />
                          <label
                            htmlFor="avatarUrl"
                            className="user-form-label-text"
                          >
                            Avatar Image
                          </label>
                          <FileUpload
                            onFileSubmitSuccess={(file) =>
                              uploadFile(file, setFieldValue)
                            }
                            name="avatarUrl"
                          ></FileUpload>
                        </div>
                        <Row>
                          <label
                            htmlFor="twoFactorEnabled"
                            className="user-form-label-text"
                          >
                            Text (SMS) Message Authentication
                          </label>
                        </Row>
                        <Col lg={6} md={12} className="card-body mb-3">
                          <ErrorMessage
                            name="twoFactorEnabled"
                            className="user-form-error"
                            component="div"
                          />
                          <label htmlFor="is2FAEnabled">
                            {" "}
                            2FA Enabled: {is2FAEnabled ? "Yes" : "No"}
                          </label>
                          <div className="form-check form-switch">
                            <Field
                              type="checkbox"
                              id="toggle"
                              checked={is2FAEnabled}
                              onChange={handleToggle}
                              className="form-check-input"
                              name="phoneNumber"
                            />
                          </div>
                          <div style={{ margin: "8px 0" }}>
                            <label htmlFor="phoneNumber">
                              Phone Number:{" "}
                              <InputGroup>
                                <InputGroup.Text>1+</InputGroup.Text>

                                <Field
                                  type="text"
                                  placeholder="Enter Phone #"
                                  className="form-control"
                                  id="phone-number"
                                  value={phoneDisplayValue.phoneNumber}
                                  onChange={onPhoneNumChange}
                                  name="phoneNumber"
                                />
                              </InputGroup>
                            </label>
                          </div>
                        </Col>
                        <Col lg={6} md={12} className="card-body mb-3">
                          <Row>
                            <div
                              style={{
                                display: "flex",
                                justifyContent: "center",
                                alignItems: "center",
                              }}
                            >
                              <Button
                                type="button"
                                onClick={onAddEditButtonSubmit}
                                variant="light"
                                size="md"
                                style={{ margin: "8px" }}
                                disabled={!user2FAData.phoneNumber}
                              >
                                {/\d/.test(user2FAData.phoneNumber)
                                  ? "Edit Phone#"
                                  : "Add Phone#"}
                              </Button>
                            </div>
                          </Row>
                          {/* <Row>
                            <div
                              style={{
                                margin: "5px 0",
                                display: "flex",
                                justifyContent: "center",
                                alignItems: "center",
                              }}
                            >
                              <Button
                                type="button"
                                onClick={onEditPhoneNumber}
                                variant="light"
                                size="md"
                                style={{ margin: "5px" }}
                                disabled={!user2FAData.phoneNumber}
                              >
                                Edit Phone#
                              </Button>
                            </div>
                          </Row> */}
                        </Col>
                        <Col lg={12} md={12} className="mb-0 d-grid gap-2">
                          <Button
                            type="route"
                            onClick={onPasswordChange}
                            variant="warning"
                          >
                            Change Password
                          </Button>
                        </Col>
                        &nbsp;
                        <div>
                          <Col lg={12} md={12} className="mb-0 d-grid gap-2">
                            <Button
                              type="submit"
                              onClick={handleSubmit}
                              variant="primary"
                            >
                              Save Changes
                            </Button>
                          </Col>
                        </div>
                      </Row>
                    </Form>
                  )}
                </Formik>
              </Card.Body>
            </Card>
          </Col>
        </Row>
      </div>
    </React.Fragment>
  );
};

UserSettingsChange.propTypes = {
  currentUser: PropTypes.shape({
    id: PropTypes.number.isRequired,
    email: PropTypes.string.isRequired,
    isLoggedIn: PropTypes.bool.isRequired,
  }).isRequired,
};

export default UserSettingsChange;
