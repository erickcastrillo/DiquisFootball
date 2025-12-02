import { useMemo } from 'react';
import { toast } from 'react-toastify';
import * as Yup from 'yup';
import { observer } from 'mobx-react-lite';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import { Button, Col, Row, Stack } from 'react-bootstrap';

import { useStore } from 'stores/store';
import type { RegisterUserRequest, ModalProps } from 'lib/types';
import { FormInput } from 'components';
import { LoadingButton, Modal } from 'components/ui';

type RegisterUserModalProps = ModalProps;

const RegisterUserModal = ({ show, onHide }: RegisterUserModalProps) => {
  const { usersStore } = useStore();
  const { createAppUser, loading } = usersStore;

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
    firstName: Yup.string().required('The first name is required'),
    lastName: Yup.string().required('The last name is required'),
    email: Yup.string().required().email(),
    password: Yup.string().required(),
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
      toast.success('User Added Successfully!');
      handleClose();
    } catch (error) {
      const message = (error as Error)?.message;
      toast.error(message);
    }
  };

  return (
    <Modal show={show} onHide={handleClose} headerTitle="Add User">
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
              label="First Name"
              placeholder="First Name"
              register={register}
              name="firstName"
            />
          </Col>

          <Col className="mt-2" xs={12} sm={6}>
            <FormInput
              type="text"
              label="Last Name"
              placeholder="Last Name"
              register={register}
              name="lastName"
            />
          </Col>

          <Col className="mt-2" xs={12} sm={6}>
            <FormInput
              type="text"
              label="Email"
              placeholder="user@email.com"
              register={register}
              name="email"
            />
          </Col>

          <Col className="mt-2" xs={12} sm={6}>
            <FormInput
              type="text"
              label="Phone Number"
              placeholder="Phone Number"
              register={register}
              name="phoneNumber"
            />
          </Col>

          <Col className="mt-2" xs={12} sm={6}>
            <FormInput
              type="text"
              label="Password"
              placeholder="Password"
              register={register}
              name="password"
            />
          </Col>

          <Col className="mt-2" xs={12}>
            <h6 className="fs-6 mb-1">Role</h6>
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
            Cancel
          </Button>
          <LoadingButton
            type="submit"
            variant="primary"
            disabled={!isDirty || !isValid || isSubmitting}
            loading={isSubmitting || loading}
          >
            Save
          </LoadingButton>
        </div>
      </form>
    </Modal>
  );
};

export default observer(RegisterUserModal);
