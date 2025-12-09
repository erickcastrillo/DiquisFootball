import { useMemo, useEffect } from 'react';
import * as Yup from 'yup';
import { observer } from 'mobx-react-lite';
import { toast } from 'react-toastify';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import { Button, Col, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';

import { useStore } from 'stores/store';
import type { Tenant, EditModalProps } from 'lib/types';
import { FormInput } from 'components';
import { LoadingButton } from 'components/ui';

type EditTenantModalProps = EditModalProps<Tenant>;

// Define validation schema - only validate editable fields
const getValidationSchema = (t: any) => Yup.object().shape({
  name: Yup.string().required(t('tenants.editModal.validation.nameRequired')),
  isActive: Yup.boolean().required()
});

const EditTenantModal = ({ show, onHide, data }: EditTenantModalProps) => {
  const { tenantsStore } = useStore();
  const { updateTenant, loading } = tenantsStore;
  const { t } = useTranslation();

  const validationSchema = useMemo(() => getValidationSchema(t), [t]);

  const defaultValues = useMemo<Tenant>(
    () => ({
      id: data.id,
      name: data.name,
      isActive: data.isActive,
      createdOn: data.createdOn,
      status: data.status,
      provisioningError: data.provisioningError,
      lastProvisioningAttempt: data.lastProvisioningAttempt
    }),
    [data]
  );

  const {
    reset,
    register,
    handleSubmit,
    formState: { isDirty, isValid, isSubmitting }
  } = useForm<Tenant>({
    defaultValues,
    resolver: yupResolver(validationSchema) as any
  });

  useEffect(() => {
    if (show) {
      reset(defaultValues);
    }
  }, [show, defaultValues, reset]);

  const handleClose = () => {
    reset();
    onHide();
  };

  const onSubmit = async (tenant: Tenant) => {
    try {
      await updateTenant(tenant);
      onHide();
      reset();
      toast.info(t('tenants.editModal.editSuccess'));
    } catch (error) {
      const message = (error as Error)?.message || t('tenants.editModal.editFailed');
      toast.error(message);
    }
  };

  return (
    <Modal show={show} onHide={handleClose} size="lg" centered>
      <Modal.Header closeButton>
        <h2 className="mb-2 fs-3">{t('tenants.editModal.editTitle')}</h2>
      </Modal.Header>
      <Modal.Body>
        <form onSubmit={handleSubmit(onSubmit)}>
          <Row className="mt-1">
            <Col className="mt-2" xs={7}>
              <FormInput
                type="text"
                placeholder={t('tenants.editModal.namePlaceholder')}
                label={t('tenants.editModal.nameLabel')}
                register={register}
                name="name"
              />
            </Col>

            <Col className="mt-2" xs={7}>
              <FormInput
                type="checkbox"
                label={t('tenants.editModal.isActiveLabel')}
                register={register}
                name="isActive"
              />
            </Col>
          </Row>

          <div className="d-flex justify-content-end mt-3 gap-2">
            <Button variant="light" onClick={onHide}>
              {t('tenants.editModal.cancel')}
            </Button>
            <LoadingButton
              type="submit"
              variant="primary"
              disabled={!isDirty || !isValid || isSubmitting}
              loading={isSubmitting || loading}
            >
              {t('tenants.editModal.save')}
            </LoadingButton>
          </div>
        </form>
      </Modal.Body>
    </Modal>
  );
};

export default observer(EditTenantModal);
