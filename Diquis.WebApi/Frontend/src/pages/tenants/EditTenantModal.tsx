import { useMemo } from 'react';
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

const EditTenantModal = ({ show, onHide, data }: EditTenantModalProps) => {
  const { tenantsStore } = useStore();
  const { updateTenant, loading } = tenantsStore;
  const { t } = useTranslation();

  const defaultValues = useMemo<Tenant>(
    () => ({
      id: data.id,
      name: data.name,
      isActive: data.isActive,
      createdOn: data.createdOn
    }),
    [data]
  );

  const validationSchema = Yup.object({
    name: Yup.string().required(t('tenants.editModal.validation.nameRequired')),
    id: Yup.string().default(data.id as string),
    isActive: Yup.boolean().default(data.isActive as boolean),
    createdOn: Yup.string().default(data.createdOn as string)
  });

  const {
    reset,
    register,
    handleSubmit,
    formState: { isDirty, isValid, isSubmitting }
  } = useForm({
    defaultValues: defaultValues,
    resolver: yupResolver(validationSchema)
  });

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
