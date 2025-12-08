import { useMemo } from 'react';
import { Col, Row, Button, Stack } from 'react-bootstrap';
import * as Yup from 'yup';
import { observer } from 'mobx-react-lite';
import { toast } from 'react-toastify';
import clsx from 'clsx';
import { Resolver, useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import { useTranslation } from 'react-i18next';

import { useStore } from 'stores/store';
import type { User, EditModalProps } from 'lib/types';
import { LoadingButton, Modal } from 'components/ui';
import { FormInput } from 'components';

type EditUserModalProps = EditModalProps<User>;

const EditUserModal = ({ show, onHide, data }: EditUserModalProps) => {
  const { usersStore } = useStore();
  const { updateAppUser, loading, loadingDelete, deleteAppUser } = usersStore;
  const { t } = useTranslation();

  const defaultValues = useMemo<User>(
    () => ({
      id: data.id,
      firstName: data.firstName,
      lastName: data.lastName,
      email: data.email,
      phoneNumber: data.phoneNumber,
      isActive: data.isActive,
      roleId: data.roleId,
      imageUrl: data.imageUrl,
      createdOn: data.createdOn
    }),
    [data]
  );

  const validationSchema = Yup.object({
    firstName: Yup.string().required(t('users.editModal.validation.firstNameRequired')),
    lastName: Yup.string().required(t('users.editModal.validation.lastNameRequired')),
    email: Yup.string().required(t('users.editModal.validation.emailRequired')).email(t('users.editModal.validation.emailInvalid')),
    phoneNumner: Yup.string(),
    roleId: Yup.string().oneOf(['admin', 'editor', 'basic']),
    isActive: Yup.boolean()
  });

  const {
    reset,
    register,
    handleSubmit,
    formState: { isDirty, isValid, isSubmitting }
  } = useForm<User>({
    defaultValues,
    resolver: yupResolver(validationSchema) as unknown as Resolver<User, any>
  });

  const handleDelete = async (id: string) => {
    try {
      await deleteAppUser(id);
      toast.error(t('users.editModal.deleteSuccess'));
      onHide();
    } catch (error) {
      const message = (error as Error)?.message || t('users.editModal.deleteFailed');
      toast.error(message);
    }
  };

  const onSubmit = async (user: User) => {
    try {
      await updateAppUser(user);
      toast.info(t('users.editModal.editSuccess'));
      onHide();
      reset();
    } catch (error) {
      const message = (error as Error)?.message || t('users.editModal.editFailed');
      toast.error(message);
    }
  };

  // check for root user to conditionally render form
  const isRootUser = data.roleId === 'root';

  return (
    <Modal show={show} onHide={onHide} headerTitle={t('users.editModal.editTitle')}>
      <form onSubmit={handleSubmit(onSubmit)}>
        <Row className="mt-1">
          {!isRootUser && (
            <Col className="mt-2" xs={12}>
              <FormInput
                type="checkbox"
                label={t('users.editModal.isActiveLabel')}
                register={register}
                name="isActive"
              />
            </Col>
          )}
          <Col className="mt-2" xs={12} sm={6}>
            <FormInput
              type="text"
              label={t('users.editModal.firstNameLabel')}
              placeholder={t('users.editModal.firstNamePlaceholder')}
              register={register}
              name="firstName"
            />
          </Col>

          <Col className="mt-2" xs={12} sm={6}>
            <FormInput
              type="text"
              label={t('users.editModal.lastNameLabel')}
              placeholder={t('users.editModal.lastNamePlaceholder')}
              register={register}
              name="lastName"
            />
          </Col>

          <Col className="mt-2" xs={12} sm={6}>
            <FormInput
              type="text"
              label={t('users.editModal.emailLabel')}
              placeholder={t('users.editModal.emailPlaceholder')}
              register={register}
              name="email"
            />
          </Col>

          <Col className="mt-2" xs={12} sm={6}>
            <FormInput
              type="text"
              label={t('users.editModal.phoneNumberLabel')}
              placeholder={t('users.editModal.phoneNumberPlaceholder')}
              register={register}
              name="phoneNumber"
            />
          </Col>

          {!isRootUser && (
            <Col className="mt-2" xs={12}>
              <h6 className="fs-6 mb-1">{t('users.editModal.roleLabel')}</h6>
              <Stack direction="horizontal" gap={1}>
                {['admin', 'editor', 'basic'].map((item) => (
                  <FormInput
                    type="radio"
                    key={item}
                    value={item}
                    label={item}
                    register={register}
                    name="roleId"
                    className="text-capitalize"
                  />
                ))}
              </Stack>
            </Col>
          )}
        </Row>

        <div
          className={clsx(
            'd-flex',
            !isRootUser ? 'justify-content-between' : 'justify-content-end',
            'mt-4'
          )}
        >
          {!isRootUser && (
            <LoadingButton
              variant="danger"
              loading={loadingDelete}
              onClick={() => handleDelete(data.id)}
            >
              {t('users.editModal.delete')}
            </LoadingButton>
          )}

          <div className="d-flex gap-2">
            <Button variant="light" onClick={onHide}>
              {t('users.editModal.cancel')}
            </Button>
            <LoadingButton
              type="submit"
              variant="primary"
              disabled={!isDirty || !isValid || isSubmitting}
              loading={isSubmitting || loading}
            >
              {t('users.editModal.save')}
            </LoadingButton>
          </div>
        </div>
      </form>
    </Modal>
  );
};

export default observer(EditUserModal);
