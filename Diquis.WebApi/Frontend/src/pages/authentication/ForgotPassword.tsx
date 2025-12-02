import { useEffect } from 'react';

import { Button } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import { yupResolver } from '@hookform/resolvers/yup';
import { useForm } from 'react-hook-form';
import { observer } from 'mobx-react-lite';
import { toast } from 'react-toastify';
import * as Yup from 'yup';

import { FormInput, AccountLayout } from 'components';
import { useStore } from 'stores/store';
import type { ForgotPasswordRequest } from 'lib/types';

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

const ForgotPassword = () => {
  const { accountStore, tenantsStore } = useStore();
  const { loadTenantOptions, tenantOptions, loading } = tenantsStore;

  const defaultValues = {
    email: '',
    password: '',
    tenant: 'root'
  };

  const validationSchema = Yup.object({
    tenant: Yup.string().required('Tenant key is required'),
    email: Yup.string()
      .email('Must be a valid email')
      .max(255)
      .required('Email is required')
  });

  const methods = useForm<ForgotPasswordRequest>({
    defaultValues,
    resolver: yupResolver(validationSchema)
  });
  const {
    handleSubmit,
    register,
    control,
    formState: { errors }
  } = methods;

  const onSubmit = async (values: ForgotPasswordRequest) => {
    const result = await accountStore.forgotPassword(values);
    if (result?.succeeded === true) {
      toast.success('Password reset email sent');
    } else {
      toast.error('Could not send email');
    }
  };

  useEffect(() => {
    loadTenantOptions();
  }, [loadTenantOptions]);

  return (
    <AccountLayout bottomLinks={<BottomLink />}>
      <div className="my-auto">
        <h4 className="mt-0">{'Forgot Password'}</h4>
        <p className="text-muted mb-4">
          {'Enter your email address to receive a password reset link.'}
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

export default observer(ForgotPassword);
