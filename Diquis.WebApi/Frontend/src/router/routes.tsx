import React, {
  Suspense,
  Fragment,
  lazy,
  LazyExoticComponent,
  FC
} from 'react';
import { Route } from 'react-router-dom';
import { Helmet } from 'react-helmet';

import { Roles } from 'lib/types';
import { AuthGuard, GuestGuard } from 'layout/guards';
import Layout from 'layout/Layout';
import LoadingScreen from 'components/LoadingScreen';

interface RouteType {
  guard?: (props: any) => JSX.Element;
  layout?: (props: any) => JSX.Element;
  component?: (props: any) => JSX.Element;
  routes?: RouteType[];
  path?: string;
  roles?: Roles[];
  title?: string;
}

const Loadable = (Component: LazyExoticComponent<FC>) => (props: any) =>
  (
    <Suspense fallback={<LoadingScreen />}>
      <Component {...props} />
    </Suspense>
  );

const NotFoundPage = Loadable(lazy(() => import('pages/404')));
const UnauthorizedPage = Loadable(lazy(() => import('pages/403')));
const LoginPage = Loadable(lazy(() => import('pages/authentication/Login')));
const ForgotPasswordPage = Loadable(
  lazy(() => import('pages/authentication/ForgotPassword'))
);
const ResetPasswordPage = Loadable(
  lazy(() => import('pages/authentication/ResetPassword'))
);
const TenantListPage = Loadable(lazy(() => import('pages/tenants/TenantList')));
const UserListPage = Loadable(lazy(() => import('pages/users/UserList')));
const ProductListPage = Loadable(lazy(() => import('pages/products/ProductList')));
const AccountSettingsPage = Loadable(
  lazy(() => import('pages/profile/AccountSettings'))
);

/* Base UI Pages */
const AccordionsPage = Loadable(
  lazy(() => import('template-resources/base-ui/Accordions'))
);
const AlertsPage = Loadable(
  lazy(() => import('template-resources/base-ui/Alerts'))
);
const AvatarsPage = Loadable(
  lazy(() => import('template-resources/base-ui/Avatars'))
);
const BadgesPage = Loadable(
  lazy(() => import('template-resources/base-ui/Badges'))
);
const ButtonsPage = Loadable(
  lazy(() => import('template-resources/base-ui/Buttons'))
);
const CardsPage = Loadable(
  lazy(() => import('template-resources/base-ui/Cards'))
);
const DropdownsPage = Loadable(
  lazy(() => import('template-resources/base-ui/Dropdowns'))
);
const FormBasicPage = Loadable(
  lazy(() => import('template-resources/base-ui/form/FormBasic'))
);
const FormAdvancedPage = Loadable(
  lazy(() => import('template-resources/base-ui/form/FormAdvanced'))
);
const ListGroupPage = Loadable(
  lazy(() => import('template-resources/base-ui/ListGroup'))
);
const ModalsPage = Loadable(
  lazy(() => import('template-resources/base-ui/Modals'))
);
const OffcanvasPage = Loadable(
  lazy(() => import('template-resources/base-ui/Offcanvas'))
);
const PaginationPage = Loadable(
  lazy(() => import('template-resources/base-ui/Pagination'))
);
const PopoversPage = Loadable(
  lazy(() => import('template-resources/base-ui/Popovers'))
);
const ProgressPage = Loadable(
  lazy(() => import('template-resources/base-ui/Progress'))
);
const SpinnersPage = Loadable(
  lazy(() => import('template-resources/base-ui/Spinners'))
);
const TablesPage = Loadable(
  lazy(() => import('template-resources/base-ui/Tables'))
);
const TabsPage = Loadable(
  lazy(() => import('template-resources/base-ui/Tabs'))
);
const TooltipsPage = Loadable(
  lazy(() => import('template-resources/base-ui/Tooltips'))
);
const TypographyPage = Loadable(
  lazy(() => import('template-resources/base-ui/Typography'))
);
const ChartsPage = Loadable(
  lazy(() => import('template-resources/extended-ui/charts'))
);
const DashboardPage = Loadable(
  lazy(() => import('template-resources/extended-ui/Dashboard'))
);
const RemixIconPage = Loadable(
  lazy(() => import('template-resources/extended-ui/icons/RemixIcon'))
);
const MaterialDesignIconPage = Loadable(
  lazy(() => import('template-resources/extended-ui/icons/MaterialDesignIcon'))
);
const UniconIconPage = Loadable(
  lazy(() => import('template-resources/extended-ui/icons/UniconIcon'))
);
const DragulaPage = Loadable(
  lazy(() => import('template-resources/extended-ui/Dragula'))
);
const RangeSlidersPage = Loadable(
  lazy(() => import('template-resources/extended-ui/RangeSliders'))
);

export const renderRoutes = (routes: RouteType[] = []) =>
  routes.map((route, i) => {
    const Guard = route.guard || React.Component;
    const Layout = route.layout || Fragment;
    const Component = route.component || React.Component;

    return (
      <Route
        key={i}
        path={route.path}
        element={
          <Fragment>
            {route.title && (
              <Helmet>
                <title>{route.title}</title>
              </Helmet>
            )}
            {route.guard ? (
              <Guard roles={route.roles}>
                <Layout>
                  <Component />
                </Layout>
              </Guard>
            ) : (
              <Layout>
                <Component />
              </Layout>
            )}
          </Fragment>
        }
      />
    );
  });

const appName = 'Nano ASP.NET Boilerplate React UI';

const routes: RouteType[] = [
  {
    path: '/',
    guard: GuestGuard,
    component: LoginPage,
    title: `Login - ${appName}`
  },
  {
    path: '/404',
    component: NotFoundPage,
    title: `Not Found - ${appName}`
  },
  {
    path: '/403',
    component: UnauthorizedPage,
    title: `Unauthorized - ${appName}`
  },
  {
    path: '/login',
    guard: GuestGuard,
    component: LoginPage,
    title: `Login - ${appName}`
  },
  {
    path: '/forgot-password',
    guard: GuestGuard,
    component: ForgotPasswordPage,
    title: `Forgot Password - ${appName}`
  },
  {
    path: '/reset-password',
    guard: GuestGuard,
    component: ResetPasswordPage,
    title: `Reset Password - ${appName}`
  },
  {
    path: '/products',
    guard: AuthGuard,
    layout: Layout,
    component: ProductListPage,
    title: `Products - ${appName}`
  },
  {
    path: '/profile',
    guard: AuthGuard,
    component: AccountSettingsPage,
    title: `Profile - ${appName}`,
    layout: Layout
  },
  {
    path: '/users',
    component: UserListPage,
    guard: AuthGuard,
    roles: [Roles.root, Roles.admin],
    title: `User Administration - ${appName}`,
    layout: Layout
  },
  {
    path: '/tenants',
    component: TenantListPage,
    guard: AuthGuard,
    roles: [Roles.root],
    title: `Tenant Administration - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/accordions',
    component: AccordionsPage,
    title: `Accordions - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/alerts',
    component: AlertsPage,
    title: `Alerts - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/avatars',
    component: AvatarsPage,
    title: `Avatars - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/badges',
    component: BadgesPage,
    title: `Badges - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/buttons',
    component: ButtonsPage,
    title: `Buttons - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/cards',
    component: CardsPage,
    title: `Cards - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/dropdowns',
    component: DropdownsPage,
    title: `Dropdowns - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/form-basic',
    component: FormBasicPage,
    title: `Form Basic - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/form-advanced',
    component: FormAdvancedPage,
    title: `Form Advanced - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/list-group',
    component: ListGroupPage,
    title: `List Group - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/modals',
    component: ModalsPage,
    title: `Modals - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/offcanvas',
    component: OffcanvasPage,
    title: `Offcanvas - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/pagination',
    component: PaginationPage,
    title: `Pagination - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/popovers',
    component: PopoversPage,
    title: `Popovers - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/progress',
    component: ProgressPage,
    title: `Progress - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/spinners',
    component: SpinnersPage,
    title: `Spinners - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/tables',
    component: TablesPage,
    title: `Tables - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/tabs',
    component: TabsPage,
    title: `Tabs - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/tooltips',
    component: TooltipsPage,
    title: `Tooltips - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/typography',
    component: TypographyPage,
    title: `Typography - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/charts',
    component: ChartsPage,
    title: `Charts - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/dashboard',
    component: DashboardPage,
    title: `Dashboard - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/icons/remix-icons',
    component: RemixIconPage,
    title: `Remix Icons - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/icons/mdi',
    component: MaterialDesignIconPage,
    title: `Material Design - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/icons/unicons',
    component: UniconIconPage,
    title: `Unicons - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/dragula',
    component: DragulaPage,
    title: `Dragula - ${appName}`,
    layout: Layout
  },
  {
    path: '/ui/range-sliders',
    component: RangeSlidersPage,
    title: `RangeSliders - ${appName}`,
    layout: Layout
  },
  {
    path: '*',
    component: NotFoundPage
  }
];

export default routes;
