import { useEffect } from "react";
import { Button } from "react-bootstrap";
import { Link } from "react-router-dom";
import { toast } from "react-toastify";
import * as Yup from "yup";
import { useForm } from "react-hook-form";
import { observer } from "mobx-react-lite";
import { yupResolver } from "@hookform/resolvers/yup";

import { FormInput } from "components";
import AccountLayout from "components/AccountLayout";
import { useStore } from "stores/store";
import type { UserLogin } from "lib/types";
import { LoadingButton } from "components/ui";

type BottomLinkProps = {
  handleAdminCredentials: () => void;
};

const BottomLink = ({ handleAdminCredentials }: BottomLinkProps) => {
  return (
    <footer className="footer footer-alt p-0">
      <p className="text-muted">
        Demo Quick Access{" "}
        <Button variant="outline-primary" className="m-2" onClick={handleAdminCredentials}>
          Admin Credentials
        </Button>
      </p>
    </footer>
  );
};

const Login = () => {
  const { accountStore, tenantsStore } = useStore();
  const { loadTenantOptions, tenantOptions, loading } = tenantsStore;

  const defaultValues = {
    email: "",
    password: "",
    tenant: "root",
  };

  const validationSchema = Yup.object({
    tenant: Yup.string().required("Tenant key is required"),
    email: Yup.string().email("Must be a valid email").max(255).required("Email is required"),
    password: Yup.string().min(6, "Password should be of minimum 6 characters length").required("Password is required"),
  });

  const methods = useForm<UserLogin>({
    defaultValues,
    resolver: yupResolver(validationSchema),
  });
  const {
    handleSubmit,
    register,
    control,
    setError,
    setValue,
    formState: { errors, isValid, isSubmitting },
  } = methods;

  // Admin Credentials button
  const handleAdminCredentials = () => {
    populateField("email", "admin@email.com");
    populateField("password", "Password123!");
    populateField("tenant", "root");
  };

  // To prepopulate a field it must be touched
  const populateField = (name: keyof UserLogin, value: string) => {
    setValue(name, value);
  };

  const onSubmit = async (values: UserLogin) => {
    try {
      await accountStore.login(values);
      toast.success("Logged In Successfully!");
    } catch (error) {
      const message = (error as Error)?.message || "Login failed";
      toast.error(message);
      setError("email", {
        message: "Wrong user credentials",
      });
      setError("password", {
        message: "Wrong user credentials",
      });
    }
  };

  useEffect(() => {
    loadTenantOptions();
  }, [loadTenantOptions]);

  return (
    <AccountLayout bottomLinks={<BottomLink handleAdminCredentials={handleAdminCredentials} />}>
      <div className="my-auto">
        <h4 className="mt-0">{"Sign In"}</h4>
        <p className="text-muted mb-4">{"Enter your email address and password to access account."}</p>

        <form onSubmit={handleSubmit(onSubmit)} noValidate>
          <FormInput label="Email" type="text" placeholder="name@example.com" containerClass={"mb-3"} register={register} name="email" />
          <FormInput label="Password" type="password" placeholder="Enter your password" containerClass={"mb-3"} register={register} name="password">
            <Link to="/forgot-password" className="text-muted float-end">
              <small>Forgot your password?</small>
            </Link>
          </FormInput>

          <div className="d-grid mb-0 text-center">
            <LoadingButton variant="primary" type="submit" disabled={!isValid && loading && isSubmitting} loading={loading || isSubmitting}>
              <div className="d-flex justify-content-center align-content-center gap-1 mx-auto">
                <i className="mdi mdi-login"></i>Log In
              </div>
            </LoadingButton>
          </div>
          <div className="mt-3">
            <small className="d-sm-flex d-none">Specify the tenant with the dropdown. The Nano Boilerplate can resolve tenants by subdomain in real deployments.</small>
            <FormInput type="select" label="Tenant" name="tenant" containerClass={"mt-1 w-50"} register={register} key="className" errors={errors} control={control}>
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

export default observer(Login);
