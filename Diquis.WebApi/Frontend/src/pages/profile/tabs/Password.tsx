import React, { useCallback, useEffect, useState } from 'react';
import * as Yup from 'yup';
import { toast } from 'react-toastify';
import { Card, Col, ProgressBar, Row } from 'react-bootstrap';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';

import { useStore } from 'stores/store';
import type { ChangePasswordRequest } from 'lib/types';
import { checkPasswordStrength } from 'utils';
import { FormInput } from 'components';
import { LoadingButton } from 'components/ui';

const Password = () => {
  const { accountStore } = useStore();
  const { changePassword } = accountStore;
  const [strength, setStrength] = useState(0);

  const defaultValues: ChangePasswordRequest = {
    password: '',
    newPassword: '',
    confirmNewPassword: ''
  };

  const fieldValidationSchema = Yup.object({
    password: Yup.string().required('Current password is required!'),
    newPassword: Yup.string().required('New password is required!'),
    confirmNewPassword: Yup.string()
      .required('Confirm new password is required!')
      .oneOf([Yup.ref('newPassword')], 'Your passwords do not match!')
  });

  const {
    register,
    handleSubmit,
    reset,
    formState: { isDirty, isValid, isSubmitting },
    watch
  } = useForm({
    defaultValues,
    resolver: yupResolver(fieldValidationSchema)
  });
  const watchedPassword = watch('newPassword');

  const handlePasswordFieldChange = useCallback(() => {
    const passwordStrength = checkPasswordStrength(watchedPassword, {
      checkFor: ['length', 'lowerCase', 'upperCase', 'number']
    }).strength;
    setStrength(passwordStrength);
  }, [watchedPassword]);

  const onSubmit = async (values: ChangePasswordRequest) => {
    try {
      await changePassword(values);
      toast.success('Password updated');
      setStrength(0);
      reset();
    } catch (error) {
      const message = (error as Error)?.message || 'Password update failed';
      toast.error(message);
    }
  };

  useEffect(() => {
    handlePasswordFieldChange();
  }, [handlePasswordFieldChange]);

  return (
    <Card className="p-2 pb-5">
      <Card.Body>
        <div className="form-block-title">Update Your Password:</div>
        <form onSubmit={handleSubmit(onSubmit)}>
          <Row>
            <Col className="mt-3" xs={12} md={6}>
              <FormInput
                type="password"
                className="w-100"
                label="Current Password"
                placeholder="Enter current password"
                register={register}
                name="password"
              />
            </Col>
            <Col className="mt-3" xs={0} md={6} />
            <Col xs={12} md={6} className="d-flex flex-column gap-1 mt-3">
              <FormInput
                type="password"
                className="w-100"
                label="New Password"
                placeholder="Enter new password"
                register={register}
                name="newPassword"
              />
              <ProgressBar now={strength} className="progress-sm" />
            </Col>
            <Col className="mt-3" xs={12} md={6}>
              <FormInput
                type="password"
                className="w-100"
                label="Confirm New Password"
                placeholder="Confirm new password"
                register={register}
                name="confirmNewPassword"
              />
            </Col>

            <Col xs={12}>
              <div className="pt-3">
                <h6 className="fs-5 fw-bolder">Password requirements:</h6>
                <small className="fw-semibold text-body-tertiary">
                  Ensure that these requirements are met:
                </small>
                <ul>
                  <li className="fs-6 text-body-tertiary">
                    Minimum 6 characters long
                  </li>
                  <li className="fs-6 text-body-tertiary">
                    At least one lowercase character
                  </li>
                  <li className="fs-6 text-body-tertiary">
                    At least one uppercase character
                  </li>
                  <li className="fs-6 text-body-tertiary">
                    At least one number
                  </li>
                </ul>
              </div>
            </Col>
            <Col xs={12} className="d-flex justify-content-end">
              <LoadingButton
                variant="primary"
                disabled={!isDirty || !isValid || isSubmitting}
                loading={isSubmitting}
                type="submit"
              >
                Save Changes
              </LoadingButton>
            </Col>
          </Row>
        </form>
      </Card.Body>
    </Card>
  );
};

export default Password;
