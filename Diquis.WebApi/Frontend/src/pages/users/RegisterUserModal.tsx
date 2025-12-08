import { useMemo } from 'react';
import { toast } from 'react-toastify';
import * as Yup from 'yup';
import { observer } from 'mobx-react-lite';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import { Button, Col, Row, Stack } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';

import { useStore } from 'stores/store';
import type { RegisterUserRequest, ModalProps } from 'lib/types';
import { FormInput } from 'components';
import { LoadingButton, Modal } from 'components/ui';

type RegisterUserModalProps = ModalProps;

const RegisterUserModal = ({ show, onHide }: RegisterUserModalProps) => {
  const { usersStore } = useStore();
  const { createAppUser, loading } = usersStore;
  const { t } = useTranslation();

  const defaultValues = useMemo<RegisterUserRequest>(
    () => ({
      id: '',
      firstName: '',
      lastName: '',
      email: '',
      password: 'Password123!',
      phoneNumber: '',
      roleId: 'basic'
    }),
    []
  );

  const validationSchema = Yup.object({
    firstName: Yup.string().required(t('users.registerModal.validation.firstNameRequired')),
    lastName: Yup.string().required(t('users.registerModal.validation.lastNameRequired')),
    email: Yup.string().required(t('users.registerModal.validation.emailRequired')).email(t('users.registerModal.validation.emailInvalid')),
    password: Yup.string().required(t('users.registerModal.validation.passwordRequired')),
    phoneNumber: Yup.string(),
    roleId: Yup.string().oneOf(['admin', 'editor', 'basic'])
  });

  const {
    reset,
    register,
    handleSubmit,
    formState: { isDirty, isValid, isSubmitting }
  } = useForm({
    defaultValues,
    resolver: yupResolver(validationSchema)
  });

  const handleClose = () => {
    reset();
    onHide();
  };

  const onSubmit = async (registerUser: RegisterUserRequest) => {
    try {
      await createAppUser(registerUser);
      toast.success(t('users.registerModal.createSuccess'));
      handleClose();
    } catch (error) {
      const message = (error as Error)?.message || t('users.registerModal.createFailed');
      toast.error(message);
    }
  };

  return (
    <Modal show={show} onHide={handleClose} headerTitle={t('users.registerModal.addTitle')}>
      <form
        onSubmit={handleSubmit(
          (user: Omit<RegisterUserRequest, 'id' | 'roleId' | 'phoneNumber'>) =>
            onSubmit({ ...defaultValues, ...user })
        )}
      >
        <Row className="mt-1">
          <Col className="mt-2" xs={12} sm={6}>
            <FormInput
              type="text"
              label={t('users.registerModal.firstNameLabel')}
              placeholder={t('users.registerModal.firstNamePlaceholder')}
              register={register}
              name="firstName"
            />
          </Col>

          <Col className="mt-2" xs={12} sm={6}>
            <FormInput
              type="text"
              label={t('users.registerModal.lastNameLabel')}
              placeholder={t('users.registerModal.lastNamePlaceholder')}
              register={register}
              name="lastName"
            />
          </Col>

          <Col className="mt-2" xs={12} sm={6}>
            <FormInput
              type="text"
              label={t('users.registerModal.emailLabel')}
              placeholder={t('users.registerModal.emailPlaceholder')}
              register={register}
              name="email"
            />
          </Col>

          <Col className="mt-2" xs={12} sm={6}>
            <FormInput
              type="text"
              label={t('users.registerModal.phoneNumberLabel')}
              placeholder={t('users.registerModal.phoneNumberPlaceholder')}
              register={register}
              name="phoneNumber"
            />
          </Col>

          <Col className="mt-2" xs={12} sm={6}>
            <FormInput
              type="text"
              label={t('users.registerModal.passwordLabel')}
              placeholder={t('users.registerModal.passwordPlaceholder')}
              register={register}
              name="password"
            />
          </Col>

          <Col className="mt-2" xs={12}>
            <h6 className="fs-6 mb-1">{t('users.registerModal.roleLabel')}</h6>
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
        </Row>

        <div className="d-flex justify-content-end mt-4 gap-2">
          <Button variant="light" onClick={() => handleClose()}>
            {t('users.registerModal.cancel')}
          </Button>
          <LoadingButton
            type="submit"
            variant="primary"
            disabled={!isDirty || !isValid || isSubmitting}
            loading={isSubmitting || loading}
          >
            {t('users.registerModal.save')}
          </LoadingButton>
        </div>
      </form>
    </Modal>
  );
};

export default observer(RegisterUserModal);
