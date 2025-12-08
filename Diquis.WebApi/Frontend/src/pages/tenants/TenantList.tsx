import { useEffect, useMemo } from 'react';
import { observer } from 'mobx-react-lite';
import { getCoreRowModel } from '@tanstack/react-table';
import { Button, Card } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';

import { useStore } from 'stores/store';
import TenantColumnShape from './TenantColumnShape';
import RegisterTenantModal from './RegisterTenantModal';
import ClientTable from 'components/tables/ClientTable/ClientTable';
import PageLayout from 'components/PageLayout';
import { useModal } from 'hooks';

// tenants list table, accessable to root admin only
const TenantList = () => {
  const { tenantsStore, authStore } = useStore();
  const { loadTenants, tenantsSorted, loadingInitial } = tenantsStore;
  const { show, onShow, onHide } = useModal();
  const { t, i18n } = useTranslation();

  useEffect(() => {
    authStore.setTitle(t('tenants.title'));
  }, [t, authStore, i18n.language]);

  const data = useMemo(() => tenantsSorted, [tenantsSorted]);
  const columns = useMemo(() => TenantColumnShape({ t }), [t]);

  useEffect(() => {
    loadTenants();
  }, [loadTenants]);

  return (
    <PageLayout
      action={
        <Button variant="primary" onClick={onShow}>
          {t('tenants.addTenant')}
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
            filterFieldPlaceholder={t('tenants.searchPlaceholder')}
          />
        </Card.Body>
      </Card>
      <div className="mb-2">
        <small>{t('tenants.paginationDescription')}</small>
      </div>
    </PageLayout>
  );
};

export default observer(TenantList);
