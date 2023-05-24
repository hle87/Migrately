import * as Yup from "yup";

const validationRegister = Yup.object().shape({
  firstName: Yup.string()
    .min(2, "Minimum 2 characters")
    .max(100, "Max 100 characters")
    .required("Required"),
  lastName: Yup.string()
    .min(2, "Minimum 2 characters")
    .max(100, "Max 100 characters")
    .required("Required"),
  mi: Yup.string().max(2, "Max 2 characters"),
  email: Yup.string()
    .email("Must be a valid email")
    .min(2, "Minimum 2 characters")
    .max(255, "Max 255 characters")
    .required("Required"),
  password: Yup.string()
    .min(8, "Minimum  characters")
    .max(100, "Max 100 characters")
    .required("Required"),
  confirmPassword: Yup.string()
    .oneOf([Yup.ref("password"), null], "Passwords must match")
    .min(8, "Minimum 8 characters")
    .max(100, "Max 100 characters")
    .required("Required"),
  avatarUrl: Yup.string()
    .url("Must be a valid url")
    .max(255, "Max 255 characters"),
  isAttorney: Yup.bool()
});

const validationLogin = Yup.object().shape({
  email: Yup.string()
    .email("Must be a valid email")
    .min(2, "Minimum 2 characters")
    .max(255, "Max 255 characters")
    .required("Required"),
  password: Yup.string()
    .min(8, "Minimum 8 characters")
    .max(100, "Max 100 characters")
    .required("Required"),
});

const validationResetPassword = Yup.object().shape({
  email: Yup.string()
    .email("Must be a valid email")
    .min(2, "Minimum 2 characters")
    .max(255, "Max 255 characters")
    .required("Required"),
});

const validationChangePassword = Yup.object().shape({
  password: Yup.string()
    .min(8, "Minimum 8 characters")
    .max(100, "Max 100 characters")
    .required("Required"),
  confirmPassword: Yup.string()
    .oneOf([Yup.ref("password"), null], "Passwords must match")
    .min(8, "Minimum 8 characters")
    .max(100, "Max 100 characters")
    .required("Required"),
});

const validationChangeUserSettings = Yup.object().shape({
  firstName: Yup.string()
    .min(2, "Minimum 2 characters")
    .max(100, "Max 100 characters")
    .required("Required"),
  lastName: Yup.string()
    .min(2, "Minimum 2 characters")
    .max(100, "Max 100 characters")
    .required("Required"),
  avatarUrl: Yup.string()
    .url("Must be a valid url")
    .max(255, "Max 255 characters"),
})

const validation2FA = Yup.object().shape({
  firstDigit: Yup.string().min(0).max(1).required("Required"),
  secondDigit: Yup.string().min(0).max(1).required("Required"),
  thirdDigit: Yup.string().min(0).max(1).required("Required"),
  fourthDigit: Yup.string().min(0).max(1).required("Required"),
  fifthDigit: Yup.string().min(0).max(1).required("Required"),
  sixthDigit: Yup.string().min(0).max(1).required("Required"),
});

const validationPhone = Yup.object().shape({
  phoneNumber: Yup.string().min(4).max(16).required("Required"),
});




export {
  validationRegister,
  validationLogin,
  validationResetPassword,
  validationChangePassword,
  validationChangeUserSettings,
  validation2FA,
  validationPhone,
};
