import React, { useState, useCallback } from "react";
import { Formik, Form, Field } from "formik";
import * as validation from "../../schemas/userSchema";
import debug from "sabio-debug";
import { Image } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import twoFAService from "services/twoFAService";
import swal from "sweetalert2";

const _logger = debug.extend("twofactorauthpage");

function TwoFactorPhoneInput() {
  const [twoFaPhoneData, setTwoFaPhoneData] = useState({ phoneNumber: "" });
  const navigate = useNavigate();

  const handleSubmit = useCallback((values) => {
    const currentPhone = values.phoneNumber;
    const stateToTransport = { type: "PHONE_NUMBER", payload: currentPhone };

    if (currentPhone) {
      setTwoFaPhoneData((prevState) => ({
        ...prevState,
        phoneNumber: currentPhone,
      }));
    }
    const handler = onPhoneSuccess(stateToTransport);
    twoFAService
      .enterPhoneNumber(values.phoneNumber)
      .then(handler)
      .catch(onPhoneFailure);
  }, []);

  const onPhoneSuccess = (stateToTransport) => {
    swal.fire("Success", "Verification Code Sent!");

    navigate("/twofactorauthpage", { state: stateToTransport });
  };

  const onPhoneFailure = (error) => {
    _logger("error response", error);
    const errMsg = error.message;
    if (errMsg === "Request failed with status code 500") {
      swal.fire(
        "Error:",
        "Phone number not found. Please contact our IT department at (555) 555-5555."
      );
      navigate("/phonenumber");
    } else {
      swal.fire("Error", "Login Failed. Please Try Again.");
      navigate("/phonenumber");
    }
  };

  return (
    <React.Fragment>
      <div className="container">
        <div className="row">
          <div className="col-lg-5 col-md-7 mx-auto my-auto">
            <div className="card">
              <div className="card-body px-lg-5 py-lg-5 text-center">
                <Image
                  src="https://icon-library.com/images/blue-phone-icon-png/blue-phone-icon-png-2.jpg"
                  className="rounded-circle avatar-lg img-thumbnail mb-4"
                  alt="profile-image"
                />
                <h2 className="text-info">2FA Security</h2>
                <p className="mb-4">
                  Enter your phone number to receive a verification code.
                </p>
                <Formik
                  enableReinitialize={true}
                  initialValues={twoFaPhoneData}
                  onSubmit={handleSubmit}
                  validationSchema={validation.validationPhone}
                >
                  <Form>
                    <div className="row mb-4">
                      <Field
                        type="text"
                        className="form-control text-lg text-center"
                        placeholder="_"
                        name="phoneNumber"
                      />
                    </div>
                    <div className="text-center">
                      <button type="submit" className="btn bg-info btn-lg my-4">
                        Send Verification Code
                      </button>
                    </div>
                  </Form>
                </Formik>
              </div>
            </div>
          </div>
        </div>
      </div>
    </React.Fragment>
  );
}

export default TwoFactorPhoneInput;
