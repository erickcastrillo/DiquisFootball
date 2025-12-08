import { useState } from 'react';
import * as Yup from 'yup';
import { observer } from 'mobx-react-lite';
import { toast } from 'react-toastify';
import { Button, Col, Row } from 'react-bootstrap';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import { PaginationState, SortingState } from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';

import { useStore } from 'stores/store';
import { AddProductRequest, Product, Locale } from 'lib/types';
import { FormInput } from 'components';
import { LoadingButton, Modal } from 'components/ui';

type ProductModalProps = {
  show: boolean;
  onHide: () => void;
  paginationState: PaginationState;
  filteredQuery: string;
  sorting: SortingState;
  isEdit?: boolean;
  data?: Product;
};

const ProductModal = ({
  show,
  onHide,
  paginationState,
  filteredQuery,
  sorting,
  isEdit = false,
  data
}: ProductModalProps) => {
  const { productsStore } = useStore();
  const { createProduct, loading, updateProduct, loadProducts } = productsStore;
  const { t, i18n } = useTranslation();

  const [newProductFormValues] = useState<Product>({
    id: isEdit ? data?.id || '' : '',
    name: isEdit ? data?.name || '' : '',
    description: isEdit ? data?.description || '' : '',
    createdOn: isEdit ? data?.createdOn || '' : '',
    locale: isEdit ? data?.locale || (i18n.language as Locale) : (i18n.language as Locale)
  });

  const validationSchema = Yup.object({
    name: Yup.string().required(t('products.modal.validation.nameRequired')),
    description: Yup.string().required(t('products.modal.validation.descriptionRequired')),
  });

  const defaultValues: Omit<AddProductRequest, 'locale'> = {
    name: newProductFormValues.name,
    description: newProductFormValues.description,
  };

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

  const onSubmit = async (formValues: Omit<AddProductRequest, 'locale'>) => {
    const addProduct: AddProductRequest = {
      ...formValues,
      locale: (i18n.language.charAt(0).toUpperCase() + i18n.language.slice(1)) as Locale,
    };
    if (!isEdit) {
      try {
        await createProduct(addProduct);
        handleClose();
        toast.success(t('products.modal.createSuccess'));
        await loadProducts({
          pageNumber: paginationState.pageIndex,
          pageSize: paginationState.pageSize,
          keyword: filteredQuery,
          sorting
        });
      } catch (error) {
        const message = (error as Error)?.message || t('products.modal.createFailed');
        toast.error(message);
      }
    } else {
      if (!data?.id) return;
      try {
        await updateProduct(addProduct, data.id);
        handleClose();
        toast.info(t('products.modal.updateSuccess'));
        await loadProducts({
          pageNumber: paginationState.pageIndex,
          pageSize: paginationState.pageSize,
          keyword: filteredQuery,
          sorting
        });
      } catch (error) {
        const message = (error as Error)?.message || t('products.modal.updateFailed');
        toast.error(message);
      }
    }
  };

  const handleDelete = async () => {
    try {
      if (!data?.id) return;
      await productsStore.deleteProduct(data.id);
      toast.error(t('products.modal.deleteSuccess'));
      await productsStore.loadProducts({
        pageNumber: 1,
        pageSize: paginationState.pageSize,
        keyword: filteredQuery,
        sorting
      });
    } catch (error) {
      const message = (error as Error)?.message || t('products.modal.updateFailed');
      toast.error(message);
    }
  };

  return (
    <Modal
      show={show}
      onHide={handleClose}
      headerTitle={isEdit ? t('products.modal.editTitle') : t('products.modal.addTitle')}
    >
      <form onSubmit={handleSubmit(onSubmit)}>
        <Row className="mt-1">
          <Col className="mt-2" xs={12}>
            <FormInput
              type="text"
              label={t('products.modal.nameLabel')}
              placeholder={t('products.modal.namePlaceholder')}
              register={register}
              name="name"
            />
          </Col>
          <Col className="mt-2" xs={12}>
            <FormInput
              type="textarea"
              rows="2"
              label={t('products.modal.descriptionLabel')}
              placeholder={t('products.modal.descriptionPlaceholder')}
              register={register}
              name="description"
            />
          </Col>
        </Row>

        <div className="d-flex w-100 my-3 gap-2">
          {isEdit && (
            <Button variant="danger" onClick={handleDelete}>
              {t('products.modal.delete')}
            </Button>
          )}
          <Button
            size="sm"
            variant="light"
            onClick={() => handleClose()}
            className="ms-auto"
          >
            {t('products.modal.cancel')}
          </Button>
          <LoadingButton
            type="submit"
            variant="primary"
            disabled={!isDirty || !isValid || isSubmitting}
            loading={isSubmitting || loading}
          >
            {t('products.modal.save')}
          </LoadingButton>
        </div>
      </form>
    </Modal>
  );
};

export default observer(ProductModal);
