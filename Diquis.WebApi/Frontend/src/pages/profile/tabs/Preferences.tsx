import { FC, useState } from 'react';
import { toast } from 'react-toastify';
import { Card, Col, Form, Row } from 'react-bootstrap';
import { useForm } from 'react-hook-form';
import { observer } from 'mobx-react-lite';

import { useStore } from 'stores/store';
import type { UpdatePreferencesRequest } from 'lib/types';
import { FormInput } from 'components';
import { LoadingButton } from 'components/ui';
import { LayoutTheme } from 'constants';

const Preferences: FC = () => {
  const { layoutStore } = useStore();

  const [preferencesFormValues, setPreferencesFormValues] =
    useState<UpdatePreferencesRequest>({
      darkModeDefault: layoutStore.theme === LayoutTheme.LAYOUT_THEME_DARK,
      pageSizeDefault: layoutStore.pageSize
    });

  const {
    register,
    handleSubmit,
    reset,
    formState: { isDirty, isValid, isSubmitting }
  } = useForm({
    defaultValues: preferencesFormValues
  });

  const onSubmit = async (values: UpdatePreferencesRequest) => {
    layoutStore.changeTheme(
      values.darkModeDefault
        ? LayoutTheme.LAYOUT_THEME_DARK
        : LayoutTheme.LAYOUT_THEME_LIGHT
    );
    layoutStore.changePageSize(values.pageSizeDefault);
    toast.success('Preferences Updated');
    setPreferencesFormValues(values);
  };

  return (
    <Card className="p-2 pb-5">
      <Card.Body>
        <div className="form-block-title">Preferences:</div>
        <form onSubmit={handleSubmit(onSubmit)}>
          <Row className="gap-4">
            <Col xs={7} mt={2}>
              <p className="fs-5 fw-bolder mb-1">Page Size Default</p>
              <FormInput
                type="select"
                className="w-25"
                register={register}
                name="pageSizeDefault"
              >
                {[5, 10, 25, 50, 100].map((item, index) => (
                  <option value={item} key={index} className="fs-6 fw-semibold">
                    {item}
                  </option>
                ))}
              </FormInput>
            </Col>
            <Col xs={7}>
              <Form.Switch label="Dark mode" {...register('darkModeDefault')} />
            </Col>
          </Row>
          <div className="d-flex justify-content-end">
            <LoadingButton
              type="submit"
              variant="primary"
              disabled={!isDirty || !isValid || isSubmitting}
              loading={isSubmitting}
            >
              Save Changes
            </LoadingButton>
          </div>
        </form>
      </Card.Body>
    </Card>
  );
};

export default observer(Preferences);
