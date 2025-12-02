import { useEffect, useMemo } from 'react';
import { observer } from 'mobx-react-lite';
import { getCoreRowModel } from '@tanstack/react-table';
import { Button, Card } from 'react-bootstrap';

import { useStore } from 'stores/store';
import TenantColumnShape from './TenantColumnShape';
import RegisterTenantModal from './RegisterTenantModal';
import ClientTable from 'components/tables/ClientTable/ClientTable';
import PageLayout from 'components/PageLayout';
import { useModal } from 'hooks';

// tenants list table, accessable to root admin only
const TenantList = () => {
  const { tenantsStore } = useStore();
  const { loadTenants, tenantsSorted, loadingInitial } = tenantsStore;
  const { show, onShow, onHide } = useModal();

  const data = useMemo(() => tenantsSorted, [tenantsSorted]);
  const columns = useMemo(() => TenantColumnShape, [TenantColumnShape]);

  useEffect(() => {
    loadTenants();
  }, [loadTenants]);

  return (
    <PageLayout
      title="Tenants"
      action={
        <Button variant="primary" onClick={onShow}>
          Add Tenant
        </Button>
      }
    >
      <RegisterTenantModal show={show} onHide={onHide} />
      <Card className="pt-2">
        <Card.Body>
          <ClientTable
            data={data}
            columns={columns}
            getCoreRowModel={getCoreRowModel()}
            isLoading={loadingInitial}
            filterFieldPlaceholder="Search tenants..."
          />
        </Card.Body>
      </Card>
      <div className="mb-2">
        <small>Client-side pagination with sorting & filtering</small>
      </div>
    </PageLayout>
  );
};

export default observer(TenantList);
