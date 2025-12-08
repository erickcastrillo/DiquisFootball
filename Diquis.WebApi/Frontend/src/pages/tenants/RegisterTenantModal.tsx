import { useMemo } from "react";
import * as Yup from "yup";
import { observer } from "mobx-react-lite";
import { toast } from "react-toastify";
import { Button, Col, Row } from "react-bootstrap";
import { useForm } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";
import { useTranslation } from "react-i18next";

import { useStore } from "stores/store";
import type { CreateTenantRequest, ModalProps } from "lib/types";
import { FormInput } from "components";
import { LoadingButton, Modal } from "components/ui";

type RegisterTenantModalProps = ModalProps;

const RegisterTenantModal = ({ show, onHide }: RegisterTenantModalProps) => {
  const { tenantsStore } = useStore();
  const { createTenant, loading } = tenantsStore;
  const { t } = useTranslation();

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
    id: Yup.string().required(t('tenants.registerModal.validation.keyRequired')),
    name: Yup.string().required(t('tenants.registerModal.validation.nameRequired')),
    adminEmail: Yup.string().required(t('tenants.registerModal.validation.adminEmailRequired')).email(t('tenants.registerModal.validation.adminEmailInvalid')),
    password: Yup.string().required(t('tenants.registerModal.validation.passwordRequired'))
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
      toast.success(t('tenants.registerModal.createSuccess'));
    } catch (error) {
      const message = (error as Error)?.message || t('tenants.registerModal.createFailed');
      toast.error(message);
    }
  };

  return (
    <Modal show={show} onHide={handleClose} headerTitle={t('tenants.registerModal.addTitle')}>
      <form onSubmit={handleSubmit(onSubmit)}>
        <Row className="mt-2">
          <Col className="mt-2" xs={6}>
            <FormInput type="text" placeholder={t('tenants.registerModal.tenantIdPlaceholder')} label={t('tenants.registerModal.tenantIdLabel')} register={register} name="id" />
          </Col>
          <Col className="mt-2" xs={6}>
            <FormInput type="text" placeholder={t('tenants.registerModal.namePlaceholder')} label={t('tenants.registerModal.nameLabel')} register={register} name="name" />
          </Col>

          <Col className="mt-2" xs={6}>
            <FormInput type="text" placeholder={t('tenants.registerModal.adminEmailPlaceholder')} label={t('tenants.registerModal.adminEmailLabel')} register={register} name="adminEmail" />
          </Col>

          <Col className="mt-2" xs={6}>
            <FormInput type="text" placeholder={t('tenants.registerModal.passwordPlaceholder')} label={t('tenants.registerModal.passwordLabel')} register={register} name="password" />
          </Col>
        </Row>
        <Row>
          <Col xs={6}>
            <FormInput type="checkbox" label={t('tenants.registerModal.isolatedDatabaseLabel')} register={register} name="hasIsolatedDatabase" />
          </Col>
        </Row>

        <div className="d-flex justify-content-end mt-4 gap-2">
          <Button variant="light" onClick={onHide}>
            {t('tenants.registerModal.cancel')}
          </Button>
          <LoadingButton type="submit" variant="primary" disabled={!isDirty || !isValid || isSubmitting} loading={isSubmitting || loading}>
            {t('tenants.registerModal.save')}
          </LoadingButton>
        </div>
      </form>
    </Modal>
  );
};

export default observer(RegisterTenantModal);
