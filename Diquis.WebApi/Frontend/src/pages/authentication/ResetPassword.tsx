import { useCallback, useEffect, useState } from 'react';
import { Button, ProgressBar } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import { yupResolver } from '@hookform/resolvers/yup';
import { toast } from 'react-toastify';
import * as Yup from 'yup';
import { useForm } from 'react-hook-form';
import { observer } from 'mobx-react-lite';

import { FormInput } from 'components';
import AccountLayout from 'components/AccountLayout';
import { useStore } from 'stores/store';
import type { ResetPasswordRequest } from 'lib/types';
import checkPasswordStrength from 'utils/checkPasswordStrength';

const BottomLink = () => {
  return (
    <footer className="footer footer-alt">
      <small className="text-muted">
        <Link to={'/login'} className="text-muted ms-1">
          {'Return to Login'}
        </Link>
      </small>
    </footer>
  );
};

const ResetPassword = () => {
  const { accountStore, tenantsStore } = useStore();
  const { loadTenantOptions, tenantOptions, loading } = tenantsStore;
  const [strength, setStrength] = useState(0);

  const defaultValues = {
    email: 'demo@example.com',
    token: '',
    password: '',
    confirmPassword: '',
    tenant: 'root'
  };

  const validationSchema = Yup.object({
    email: Yup.string()
      .email('Must be a valid email')
      .max(255)
      .required('Email is required'),
    token: Yup.string().required('Token is required!'),
    password: Yup.string().required('New password is required!'),
    confirmPassword: Yup.string().required('Confirm new password is required!'),
    tenant: Yup.string().required('Tenant key is required')
  });

  const methods = useForm<ResetPasswordRequest>({
    defaultValues,
    resolver: yupResolver(validationSchema)
  });
  const {
    handleSubmit,
    register,
    control,
    formState: { errors },
    watch
  } = methods;
  const watchedPassword = watch('password');

  const onSubmit = async (values: ResetPasswordRequest) => {
    const result = await accountStore.forgotPassword(values);
    if (result?.succeeded === true) {
      toast.success('Password reset email sent');
    } else {
      toast.error('Could not send email');
    }
  };

  const handleNewPasswordChange = useCallback(() => {
    const passwordStrength = checkPasswordStrength(watchedPassword, {
      checkFor: [
        'length',
        'lowerCase',
        'upperCase',
        'number',
        'specialCharacter'
      ]
    }).strength;
    setStrength(passwordStrength);
  }, [watchedPassword]);

  useEffect(() => {
    handleNewPasswordChange();
  }, [handleNewPasswordChange]);

  useEffect(() => {
    loadTenantOptions();
  }, [loadTenantOptions]);

  return (
    <AccountLayout bottomLinks={<BottomLink />}>
      <div className="my-auto">
        <h4 className="mt-0">{'Reset Password'}</h4>
        <p className="text-muted mb-4">
          {
            'Enter your email with new password and reset token to regain access to your account.'
          }
        </p>


        <form onSubmit={handleSubmit(onSubmit)} noValidate>
          <FormInput
            label="Email"
            type="text"
            placeholder="name@example.com"
            containerClass={'mb-3'}
            register={register}
            name="email"
          />
          <div className="mb-3">
            <FormInput
              label="Password"
              type="password"
              placeholder="Password"
              register={register}
              name="password"
            />
            <ProgressBar now={strength} className="progress-sm mt-1" />
          </div>

          <FormInput
            label="Confirm Password"
            type="password"
            placeholder="Password"
            containerClass={'mb-3'}
            register={register}
            name="confirmPassword"
          />

          <FormInput
            label="Token"
            type="text"
            placeholder="Enter token from Email"
            containerClass={'mb-3'}
            register={register}
            name="token"
          />

          <div className="d-grid mb-0 text-center">
            <Button variant="primary" type="submit" disabled={loading}>
              Submit
            </Button>
          </div>
          <div className="mt-3">
            <small className="d-sm-flex d-none">
              Specify the tenant with the dropdown. The Nano Boilerplate can
              resolve tenants by subdomain in real deployments.
            </small>
            <FormInput
              type="select"
              label="Tenant"
              name="tenant"
              containerClass={'mt-1 w-50'}
              register={register}
              key="className"
              errors={errors}
              control={control}
            >
              {tenantOptions.map((item) => (
                <option key={item.id} value={item.id}>
                  {item.name}
                </option>
              ))}
            </FormInput>
          </div>
        </form>
      </div>
    </AccountLayout>
  );
};

export default observer(ResetPassword);
