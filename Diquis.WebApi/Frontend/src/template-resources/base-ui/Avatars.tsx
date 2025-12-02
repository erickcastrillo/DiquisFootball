import React from 'react';
import { Col, Row } from 'react-bootstrap';

import PageLayout from 'components/PageLayout';
import SampleWrapper from 'template-resources/SampleWrapper';
import { Avatar } from 'components/ui';
import avatar2 from 'assets/images/users/avatar-2.jpg';
import avatar3 from 'assets/images/users/avatar-3.jpg';
import avatar4 from 'assets/images/users/avatar-4.jpg';
import avatar5 from 'assets/images/users/avatar-5.jpg';
import avatar6 from 'assets/images/users/avatar-6.jpg';
import avatar7 from 'assets/images/users/avatar-7.jpg';
import avatar8 from 'assets/images/users/avatar-8.jpg';
import avatar9 from 'assets/images/users/avatar-9.jpg';
import small2 from 'assets/images/small/small-2.jpg';
import small3 from 'assets/images/small/small-3.jpg';

const Avatars = () => {
  return (
    <PageLayout title="Avatars">
      <Row>
        {/* Sizing - Images */}
        <Col xxl="6">
          <SampleWrapper
            title="Sizing - Images"
            subText={
              <>
                {' '}
                Create and group avatars of different sizes and shapes with the
                css classes. Using Bootstrap's naming convention, you can
                control size of avatar including standard avatar, or scale it up
                to different sizes.{' '}
              </>
            }
          >
            <Row>
              <Col md="3">
                <Avatar src={avatar2} text="avatar" size="xs" rounded="2" />
                <p>
                  Using <code>.avatar-xs</code>
                </p>
                <Avatar src={avatar3} text="avatar" size="sm" rounded="2" />
                <p className="mb-2 mb-sm-0">
                  Using <code>.avatar-sm</code>
                </p>
              </Col>
              <Col md="3">
                <Avatar src={avatar4} text="avatar" size="md" rounded="2" />
                <p>
                  Using <code>.avatar-md</code>
                </p>
              </Col>

              <Col md="3">
                <Avatar src={avatar5} text="avatar" size="lg" rounded="2" />
                <p>
                  Using <code>.avatar-lg</code>
                </p>
              </Col>

              <Col md="3">
                <Avatar src={avatar6} text="avatar" size="xl" rounded="2" />
                <p className="mb-0">
                  Using <code>.avatar-xl</code>
                </p>
              </Col>
            </Row>
          </SampleWrapper>
        </Col>
        {/* Rounded Circle */}
        <Col xxl="6">
          <SampleWrapper
            title="Rounded Circle"
            subText={
              <>
                Using an additional class <code>.rounded-circle</code> in{' '}
                <code>&lt;img&gt;</code> element creates the rounded avatar.
              </>
            }
          >
            <Row>
              <Col md="4">
                <Avatar src={avatar7} text="avatar" size="md" />
                <p>
                  Using <code>.avatar-md .rounded-circle</code>
                </p>
              </Col>

              <Col md="4">
                <Avatar src={avatar8} text="avatar" size="lg" />
                <p>
                  Using <code>.avatar-lg .rounded-circle</code>
                </p>
              </Col>
              <Col md="4">
                <Avatar src={avatar9} text="avatar" size="xl" />
                <p>
                  Using <code>.avatar-xl .rounded-circle</code>
                </p>
              </Col>
            </Row>
          </SampleWrapper>
        </Col>
      </Row>
      <Row>
        {/* Sizing - Background Color */}
        <Col xxl="6">
          <SampleWrapper
            title="Sizing - Background Color"
            subText={
              <>
                Using utilities classes of background e.g. <code>bg-*</code>{' '}
                allows you to have any background color as well.
              </>
            }
          >
            <Row>
              <Col md="3">
                <Avatar src="" text="xs" size="xs" rounded="2" />
                <p>
                  Using <code>.avatar-xs</code>
                </p>
                <Avatar
                  src=""
                  text="sm"
                  size="sm"
                  rounded="2"
                  textDivClassName="bg-success"
                />
                <p className="mb-2 mb-sm-0">
                  Using <code>.avatar-sm</code>
                </p>
              </Col>
              <Col md="3">
                <Avatar
                  src=""
                  text="md"
                  size="md"
                  rounded="2"
                  textDivClassName="bg-info-lighten text-info font-20"
                />
                <p>
                  Using <code>.avatar-md</code>
                </p>
              </Col>

              <Col md="3">
                <Avatar
                  src=""
                  text="lg"
                  size="lg"
                  rounded="2"
                  textDivClassName="bg-danger font-22"
                />
                <p>
                  Using <code>.avatar-lg</code>
                </p>
              </Col>

              <Col md="3">
                <Avatar
                  src=""
                  text="xl"
                  size="xl"
                  rounded="2"
                  textDivClassName="bg-warning-lighten text-warning font-24"
                />
                <p className="mb-0">
                  Using <code>.avatar-xl</code>
                </p>
              </Col>
            </Row>
          </SampleWrapper>
        </Col>
        {/* Rounded Circle Background */}
        <Col xxl="6">
          <SampleWrapper
            title="Rounded Circle Background"
            subText={
              <>
                Using an additional class <code>.rounded-circle</code> in{' '}
                <code>&lt;img&gt;</code> element creates the rounded avatar.
              </>
            }
          >
            <Row>
              <Col md="4">
                <Avatar
                  src=""
                  text="MD"
                  size="md"
                  textDivClassName="bg-secondary-lighten text-secondary font-20"
                />
                <p>
                  Using <code>.avatar-md .rounded-circle</code>
                </p>
              </Col>

              <Col md="4">
                <Avatar
                  src=""
                  text="LG"
                  size="lg"
                  textDivClassName="bg-light text-dark font-22"
                />
                <p>
                  Using <code>.avatar-lg .rounded-circle</code>
                </p>
              </Col>

              <Col md="4">
                <Avatar
                  src=""
                  text="XL"
                  size="xl"
                  textDivClassName="bg-primary-lighten text-primary font-24"
                />
                <p className="mb-0">
                  Using <code>.avatar-xl .rounded-circle</code>
                </p>
              </Col>
            </Row>
          </SampleWrapper>
        </Col>
      </Row>
      <Row>
        {/* Images Shapes */}
        <Col xs="12">
          <SampleWrapper
            title="Images Shapes"
            subText={<> Avatars with different sizes and shapes. </>}
          >
            <Row>
              <Col sm="2">
                <Avatar
                  src={small2}
                  text="avatar"
                  className="img-fluid rounded"
                  width="150px"
                  height="150px"
                />
                <p className="mb-0">
                  <code>.rounded</code>
                </p>
              </Col>

              <Col sm="2" className="text-center">
                <Avatar
                  src={avatar6}
                  text="avatar"
                  className="img-fluid rounded"
                  width="120px"
                  height="120px"
                />
                <p className="mb-0">
                  <code>.rounded</code>
                </p>
              </Col>

              <Col sm="2" className="text-center">
                <Avatar
                  src={avatar7}
                  text="avatar"
                  className="img-fluid"
                  width="120px"
                  height="120px"
                />
                <p className="mb-0">
                  <code>.rounded-circle</code>
                </p>
              </Col>

              <Col sm="2">
                <Avatar
                  src={small3}
                  text="avatar"
                  className="img-fluid img-thumbnail"
                  rounded="0"
                  width="200px"
                  height="200px"
                />
                <p className="mb-0">
                  <code>.img-thumbnail</code>
                </p>
              </Col>
              <Col sm="2">
                <Avatar
                  src={avatar8}
                  text="avatar"
                  className="img-fluid img-thumbnail"
                  rounded="0"
                  width="120px"
                  height="120px"
                />
                <p className="mb-0">
                  <code>.rounded-circle .img-thumbnail</code>
                </p>
              </Col>
            </Row>
          </SampleWrapper>
        </Col>
      </Row>
    </PageLayout>
  );
};

export default Avatars;
