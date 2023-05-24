import React, { useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { Formik, Form, Field } from "formik";
import * as validation from "../../schemas/userSchema";
import debug from "sabio-debug";
import { Image } from "react-bootstrap";
import twoFAService from "services/twoFAService";
import swal from "sweetalert2";

const _logger = debug.extend("twofactorauthpage");

function TwoFactorAuthInput() {
  const [twoFaDigit] = useState({
    firstDigit: "",
    secondDigit: "",
    thirdDigit: "",
    fourthDigit: "",
    fifthDigit: "",
    sixthDigit: "",
  });

  const navigate = useNavigate();

  const { state } = useLocation();

  const handleSubmit = (values) => {
    let sixDigitCode = Object.keys(values)
      .map((objectKey) => values[objectKey])
      .join("");

    twoFAService
      .verifyUserCode(state.payload, sixDigitCode)
      .then(onVerifySuccess)
      .catch(onVerifyFailure);
  };

  const onVerifySuccess = (response) => {
    if (response.item === "approved") {
      swal.fire("Success", "Verification Code Accepted!").then((result) => {
        if (result.isConfirmed) {
          navigate("/");
        }
      });
    } else {
      swal.fire("Error", "Verification Code Not Accepted!");
      navigate("/twofactorauthpage");
    }
  };

  const onVerifyFailure = (err) => {
    _logger("error", err);
    swal.fire("Error", "Verification Code Not Accepted!");
    navigate("/twofactorauthpage");
  };

  return (
    <React.Fragment>
      <div className="container">
        <div className="row">
          <div className="col-lg-5 col-md-7 mx-auto my-auto">
            <div className="card">
              <div className="card-body px-lg-5 py-lg-5 text-center">
                <Image
                  src="https://i.pinimg.com/736x/89/d0/a7/89d0a732359dee26a70718cf48736c47.jpg"
                  className="rounded-circle avatar-lg img-thumbnail mb-4"
                  alt="profile-image"
                />
                <h2 className="text-info">2FA Security</h2>
                <p className="mb-4">
                  Enter 6-digits code from your athenticatior app.
                </p>
                <Formik
                  enableReinitialize={true}
                  initialValues={twoFaDigit}
                  onSubmit={handleSubmit}
                  validationSchema={validation.validation2FA}
                >
                  <Form>
                    <div className="row mb-4">
                      <div className="col-lg-2 col-md-2 col-2 ps-0 ps-md-2">
                        <Field
                          type="text"
                          maxlength="1"
                          className="form-control text-lg text-center"
                          placeholder="_"
                          name="firstDigit"
                        />
                      </div>
                      <div className="col-lg-2 col-md-2 col-2 ps-0 ps-md-2">
                        <Field
                          type="text"
                          maxlength="1"
                          className="form-control text-lg text-center"
                          placeholder="_"
                          name="secondDigit"
                        />
                      </div>
                      <div className="col-lg-2 col-md-2 col-2 ps-0 ps-md-2">
                        <Field
                          type="text"
                          maxlength="1"
                          className="form-control text-lg text-center"
                          placeholder="_"
                          name="thirdDigit"
                        />
                      </div>
                      <div className="col-lg-2 col-md-2 col-2 pe-0 pe-md-2">
                        <Field
                          type="text"
                          maxlength="1"
                          className="form-control text-lg text-center"
                          placeholder="_"
                          name="fourthDigit"
                        />
                      </div>
                      <div className="col-lg-2 col-md-2 col-2 pe-0 pe-md-2">
                        <Field
                          type="text"
                          maxlength="1"
                          className="form-control text-lg text-center"
                          placeholder="_"
                          name="fifthDigit"
                        />
                      </div>
                      <div className="col-lg-2 col-md-2 col-2 pe-0 pe-md-2">
                        <Field
                          type="text"
                          maxlength="1"
                          className="form-control text-lg text-center"
                          placeholder="_"
                          name="sixthDigit"
                        />
                      </div>
                    </div>
                    <div className="text-center">
                      <button type="submit" className="btn bg-info btn-lg my-4">
                        Continue
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

export default TwoFactorAuthInput;
