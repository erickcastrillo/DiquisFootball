import { FC, useEffect, useState } from 'react';
import * as Yup from 'yup';
import { observer } from 'mobx-react-lite';
import { toast } from 'react-toastify';
import { Card, Col, Row } from 'react-bootstrap';
import { Resolver, useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';

import { useStore } from 'stores/store';
import type {
  ChangeProfileImageRequest,
  UpdateProfileRequest
} from 'lib/types';
import { getBase64 } from 'utils';
import ImagePopover from './ImagePopover';
import { FormInput } from 'components';
import { LoadingButton } from 'components/ui';

type UserInfoSchema = Omit<UpdateProfileRequest, 'id' | 'phoneNumber'> & {
  phoneNumber?: string;
};

const UserInfo = () => {
  const {
    accountStore: { currentUser, updateCurrentUser, updateProfileImage }
  } = useStore();
  const [tempImage, setTempImage] = useState(currentUser?.imageUrl);

  const [userFormValues, setUserFormValues] = useState<UpdateProfileRequest>({
    id: currentUser?.id || '',
    firstName: currentUser?.firstName || '',
    lastName: currentUser?.lastName || '',
    phoneNumber: currentUser?.phoneNumber || '',
    email: currentUser?.email || ''
  });

  const fieldValidationSchema = Yup.object({
    firstName: Yup.string()
      .min(3, 'Too Short')
      .required('First Name is Required!'),
    lastName: Yup.string().required('Last Name is Required!'),
    email: Yup.string().required('The email is required').email(),
    phoneNumber: Yup.string().optional()
  });

  const defaultValues: UserInfoSchema = {
    firstName: userFormValues.firstName || '',
    lastName: userFormValues.lastName || '',
    email: userFormValues.email || '',
    phoneNumber: userFormValues.phoneNumber || ''
  };

  const {
    register,
    handleSubmit,
    reset,
    formState: { isDirty, isValid, isSubmitting }
  } = useForm<UserInfoSchema>({
    defaultValues,
    resolver: yupResolver(fieldValidationSchema) as Resolver<
      UserInfoSchema,
      any
    >
  });

  const onSubmit = async (values: UserInfoSchema) => {
    try {
      await updateCurrentUser({
        ...userFormValues,
        ...values
      });
      toast.success('Profile Updated');
      reset();
      setUserFormValues((prev) => ({ ...prev, ...values }));
    } catch (error) {
      const message = (error as Error)?.message || 'Update profile failed';
      toast.error(message);
    }
  };

  const handleImageUpload: React.ChangeEventHandler<HTMLInputElement> = async (
    evt
  ) => {
    try {
      const file = evt.target?.files?.[0];
      const imageRequest: ChangeProfileImageRequest = {
        ImageFile: file
      };
      await updateProfileImage(imageRequest);
      const base64 = (await getBase64(file!)) as string;
      if (base64) setTempImage(base64);
    } catch (error) {
      const message = (error as Error)?.message || 'Upload image failed';
      toast.error(message);
    }
  };

  const handleImageRemove = async () => {
    const imageRequest: ChangeProfileImageRequest = {
      DeleteCurrentImage: true
    };
    await updateProfileImage(imageRequest);
    setTempImage('');
  };

  useEffect(() => {
    if (!currentUser) return;
    reset({
      email: currentUser.email,
      firstName: currentUser.firstName,
      lastName: currentUser.lastName,
      phoneNumber: currentUser.phoneNumber || ''
    });
  }, [currentUser]);

  return (
    <Card className="p-2 pb-5">
      <Card.Body>
        <div className="form-block-title">Edit your account information:</div>
        <div>
          <div className="profile-avatar profile-avatar--large mb-2">
            <div className="profile-avatar__image-area">
              {tempImage || currentUser?.imageUrl ? (
                <img
                  src={tempImage || currentUser?.imageUrl}
                  id="profileImage"
                />
              ) : (
                <i className="mdi mdi-account-outline" />
              )}
            </div>
            <ImagePopover
              handleImageUpdate={handleImageUpload}
              handleImageRemove={handleImageRemove}
            />
            <div className="profile-avatar__title-group">
              <div
                className="profile-avatar__title-group__title text-capitalize"
                id="nameDisplay"
              >
                {currentUser?.firstName} {currentUser?.lastName}
              </div>
              <div
                className="profile-avatar__title-group__sub text-capitalize"
                id="roleDisplay"
              >
                {currentUser?.roleId} Level User
              </div>
            </div>
          </div>
        </div>
        <form onSubmit={handleSubmit(onSubmit)}>
          <Row className="mt-2">
            <Col className="mt-3" xs={12} sm={6}>
              <FormInput
                label="First Name"
                placeholder="First Name"
                register={register}
                name="firstName"
              />
            </Col>
            <Col className="mt-3" xs={12} sm={6}>
              <FormInput
                label="Last Name"
                placeholder="Last Name"
                register={register}
                name="lastName"
              />
            </Col>
            <Col className="mt-3" xs={12} sm={6}>
              <FormInput
                label="Email"
                placeholder="Email"
                register={register}
                name="email"
              />
            </Col>
            <Col className="mt-3" xs={12} sm={6}>
              <FormInput
                label="Phone number"
                placeholder="Phone number"
                register={register}
                name="phoneNumber"
              />
            </Col>
            <Col xs={12} className="d-flex justify-content-end mt-4">
              <LoadingButton
                type="submit"
                variant="primary"
                disabled={!isDirty || !isValid || isSubmitting}
                loading={isSubmitting}
              >
                Save Changes
              </LoadingButton>
            </Col>
          </Row>
        </form>
      </Card.Body>
    </Card>
  );
};

export default observer(UserInfo);
