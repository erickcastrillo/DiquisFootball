import { useEffect, useMemo } from "react";
import { observer } from "mobx-react-lite";
import { getCoreRowModel } from "@tanstack/react-table";
import { Button, Card } from "react-bootstrap";
import { useTranslation } from "react-i18next";

import { useStore } from "stores/store";
import ClientTable from "components/tables/ClientTable/ClientTable";
import UserColumnShape from "./UserColumnShape";
import PageLayout from "components/PageLayout";
import { useModal } from "hooks";
import RegisterUserModal from "./RegisterUserModal";

const UserList = () => {
  const { usersStore, authStore } = useStore();
  const { loadAppUsers, appUsersSorted, loadingInitial } = usersStore;
  const { show, onShow, onHide } = useModal();
  const { t, i18n } = useTranslation();

  useEffect(() => {
    authStore.setTitle(t('users.title'));
  }, [t, authStore, i18n.language]);

  const data = useMemo(() => appUsersSorted, [appUsersSorted]);
  const columns = useMemo(() => UserColumnShape({ t }), [t]);

  useEffect(() => {
    loadAppUsers();
  }, [loadAppUsers]);

  return (
    <PageLayout
      action={
        <Button variant="primary" onClick={onShow}>
          {t('users.newUser')}
        </Button>
      }
    >
      {show && <RegisterUserModal show={show} onHide={onHide} />}
      <Card className="pt-2">
        <Card.Body>
          <ClientTable data={data} columns={columns} getCoreRowModel={getCoreRowModel()} isLoading={loadingInitial} filterFieldPlaceholder={t('users.searchPlaceholder')} />
        </Card.Body>
      </Card>
      <div className="mb-2">
        <small>{t('users.paginationDescription')}</small>
      </div>
    </PageLayout>
  );
};

export default observer(UserList);
