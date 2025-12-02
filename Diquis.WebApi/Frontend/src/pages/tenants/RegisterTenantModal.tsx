import { useMemo } from "react";
import * as Yup from "yup";
import { observer } from "mobx-react-lite";
import { toast } from "react-toastify";
import { Button, Col, Row } from "react-bootstrap";
import { useForm } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";

import { useStore } from "stores/store";
import type { CreateTenantRequest, ModalProps } from "lib/types";
import { FormInput } from "components";
import { LoadingButton, Modal } from "components/ui";

type RegisterTenantModalProps = ModalProps;

const RegisterTenantModal = ({ show, onHide }: RegisterTenantModalProps) => {
  const { tenantsStore } = useStore();
  const { createTenant, loading } = tenantsStore;

  const defaultValues = useMemo<CreateTenantRequest>(
    () => ({
      id: "",
      name: "",
      adminEmail: "",
      password: "Password123!",
      hasIsolatedDatabase: false,
    }),
    []
  );

  const validationSchema = Yup.object({
    id: Yup.string().required("The key is required"),
    name: Yup.string().required("Tenant name is required"),
    adminEmail: Yup.string().required("The admin email is required").email(),
    password: Yup.string().required("The password is required")

  });

  const {
    reset,
    register,
    handleSubmit,
    formState: { isDirty, isValid, isSubmitting },
  } = useForm({
    defaultValues,
    resolver: yupResolver(validationSchema),
  });

  const handleClose = () => {
    reset();
    onHide();
  };

  const onSubmit = async (createTenantRequest: CreateTenantRequest) => {
    try {
      await createTenant(createTenantRequest);
      reset();
      onHide();
      toast.success("Tenant created successfully");
    } catch (error) {
      const message = (error as Error)?.message;
      toast.error(message);
    }
  };

  return (
    <Modal show={show} onHide={handleClose} headerTitle="Add Tenant">
      <form onSubmit={handleSubmit(onSubmit)}>
        <Row className="mt-2">
          <Col className="mt-2" xs={6}>
            <FormInput type="text" placeholder="Tenant Id" label="Tenant Id" register={register} name="id" />
          </Col>
          <Col className="mt-2" xs={6}>
            <FormInput type="text" placeholder="Name" label="Name" register={register} name="name" />
          </Col>

          <Col className="mt-2" xs={6}>
            <FormInput type="text" placeholder="admin@email.com" label="Admin Email" register={register} name="adminEmail" />
          </Col>

          <Col className="mt-2" xs={6}>
            <FormInput type="text" placeholder="Password" label="Password" register={register} name="password" />
          </Col>
        </Row>
        <Row>
          <Col xs={6}>
            <FormInput type="checkbox" label="Isolated Database" register={register} name="hasIsolatedDatabase" />
          </Col>
        </Row>

        <div className="d-flex justify-content-end mt-4 gap-2">
          <Button variant="light" onClick={onHide}>
            Cancel
          </Button>
          <LoadingButton type="submit" variant="primary" disabled={!isDirty || !isValid || isSubmitting} loading={isSubmitting || loading}>
            Save
          </LoadingButton>
        </div>
      </form>
    </Modal>
  );
};

export default observer(RegisterTenantModal);
