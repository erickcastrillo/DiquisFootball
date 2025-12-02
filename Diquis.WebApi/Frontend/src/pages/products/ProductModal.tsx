import { useState } from 'react';
import * as Yup from 'yup';
import { observer } from 'mobx-react-lite';
import { toast } from 'react-toastify';
import { Button, Col, Row } from 'react-bootstrap';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import { PaginationState, SortingState } from '@tanstack/react-table';

import { useStore } from 'stores/store';
import type { AddProductRequest, Product } from 'lib/types';
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

  const [newProductFormValues] = useState<Product>({
    id: isEdit ? data?.id || '' : '',
    name: isEdit ? data?.name || '' : '',
    description: isEdit ? data?.description || '' : '',
    createdOn: isEdit ? data?.createdOn || '' : ''
  });

  const validationSchema = Yup.object({
    name: Yup.string().required('Product name is required'),
    description: Yup.string().required('Description is required')
  });

  const defaultValues: AddProductRequest = {
    name: newProductFormValues.name,
    description: newProductFormValues.description
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

  const onSubmit = async (addProduct: AddProductRequest) => {
    if (!isEdit) {
      try {
        await createProduct(addProduct);
        handleClose();
        toast.success('Product Added Successfully!');
        await loadProducts({
          pageNumber: paginationState.pageIndex,
          pageSize: paginationState.pageSize,
          keyword: filteredQuery,
          sorting
        });
      } catch (error) {
        const message = (error as Error)?.message || 'Create product failed';
        toast.error(message);
      }
    } else {
      if (!data?.id) return;
      try {
        await updateProduct(addProduct, data.id);
        handleClose();
        toast.info('Product Updated Successfully!');
        await loadProducts({
          pageNumber: paginationState.pageIndex,
          pageSize: paginationState.pageSize,
          keyword: filteredQuery,
          sorting
        });
      } catch (error) {
        const message = (error as Error)?.message || 'Update product failed';
        toast.error(message);
      }
    }
  };

  const handleDelete = async () => {
    try {
      if (!data?.id) return;
      await productsStore.deleteProduct(data.id);
      toast.error('Product Deleted Successfully!');
      await productsStore.loadProducts({
        pageNumber: 1,
        pageSize: paginationState.pageSize,
        keyword: filteredQuery,
        sorting
      });
    } catch (error) {
      const message = (error as Error)?.message || 'Update product failed';
      toast.error(message);
    }
  };

  return (
    <Modal
      show={show}
      onHide={handleClose}
      headerTitle={isEdit ? 'Edit Product' : 'Add Product'}
    >
      <form onSubmit={handleSubmit(onSubmit)}>
        <Row className="mt-1">
          <Col className="mt-2" xs={12} sm={8}>
            <FormInput
              type="text"
              label="Name"
              placeholder="Enter Product Name"
              register={register}
              name="name"
            />
          </Col>
          <Col className="mt-2" xs={12}>
            <FormInput
              type="textarea"
              rows="2"
              label="Description"
              placeholder="Enter Product Description"
              register={register}
              name="description"
            />
          </Col>
        </Row>

        <div className="d-flex w-100 my-3 gap-2">
          {isEdit && (
            <Button variant="danger" onClick={handleDelete}>
              Delete
            </Button>
          )}
          <Button
            size="sm"
            variant="light"
            onClick={() => handleClose()}
            className="ms-auto"
          >
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

export default observer(ProductModal);
