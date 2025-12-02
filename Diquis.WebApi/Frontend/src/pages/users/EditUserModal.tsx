import { useMemo } from 'react';
import { Col, Row, Button, Stack } from 'react-bootstrap';
import * as Yup from 'yup';
import { observer } from 'mobx-react-lite';
import { toast } from 'react-toastify';
import clsx from 'clsx';
import { Resolver, useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';

import { useStore } from 'stores/store';
import type { User, EditModalProps } from 'lib/types';
import { LoadingButton, Modal } from 'components/ui';
import { FormInput } from 'components';

type EditUserModalProps = EditModalProps<User>;

const EditUserModal = ({ show, onHide, data }: EditUserModalProps) => {
  const { usersStore } = useStore();
  const { updateAppUser, loading, loadingDelete, deleteAppUser } = usersStore;

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
    firstName: Yup.string().required('The first name is required'),
    lastName: Yup.string().required('The last name is required'),
    email: Yup.string().required().email(),
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
      toast.error('User Deleted Successfully!');
      onHide();
    } catch (error) {
      const message = (error as Error)?.message;
      toast.error(message);
    }
  };

  const onSubmit = async (user: User) => {
    try {
      await updateAppUser(user);
      toast.info('User Edited Successfully!');
      onHide();
      reset();
    } catch (error) {
      const message = (error as Error)?.message;
      toast.error(message);
    }
  };

  // check for root user to conditionally render form
  const isRootUser = data.roleId === 'root';

  return (
    <Modal show={show} onHide={onHide} headerTitle="Edit User">
      <form onSubmit={handleSubmit(onSubmit)}>
        <Row className="mt-1">
          {!isRootUser && (
            <Col className="mt-2" xs={12}>
              <FormInput
                type="checkbox"
                label="Is Active"
                register={register}
                name="isActive"
              />
            </Col>
          )}
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

          {!isRootUser && (
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
              Delete
            </LoadingButton>
          )}

          <div className="d-flex gap-2">
            <Button variant="light" onClick={onHide}>
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
        </div>
      </form>
    </Modal>
  );
};

export default observer(EditUserModal);
