import {
  Row,
  Col,
  Card,
  Form,
  FloatingLabel,
  InputGroup,
  Button,
  Dropdown,
  DropdownButton
} from 'react-bootstrap';

import {
  PasswordInput,
  TextAreaInput,
  TextInput,
  Form as RHForm,
  SelectInput,
  FileInput,
  DateInput,
  ColorInput
} from 'components';
import PageLayout from 'components/PageLayout';

const BasicInputElements = () => {
  return (
    <Card>
      <Card.Body>
        <h4 className="header-title">Input Types</h4>
        <p className="text-muted">
          Most common form control, text-based input fields. Includes support
          for all HTML5 types: <code>text</code>, <code>password</code>,{' '}
          <code>datetime</code>,<code>datetime-local</code>, <code>date</code>,{' '}
          <code>month</code>,<code>time</code>, <code>week</code>,{' '}
          <code>number</code>, <code>email</code>,<code>url</code>,{' '}
          <code>search</code>, <code>tel</code>, and <code>color</code>.
        </p>

        <Row>
          <Col lg={6}>
            <RHForm onSubmit={() => {}}>
              <TextInput
                label="Text"
                type="text"
                name="text"
                containerClass={'mb-3'}
                key="text"
              />

              <TextInput
                label="Email"
                type="email"
                name="email"
                placeholder="Email"
                containerClass={'mb-3'}
                key="email"
              />

              <PasswordInput
                label="Show/Hide Password"
                name="password"
                placeholder="password placeholder"
                containerClass={'mb-3'}
                key="password"
              />

              <TextInput
                label="Placeholder"
                type="text"
                name="placeholder"
                placeholder="placeholder"
                containerClass={'mb-3'}
                key="placeholder"
              />

              <TextAreaInput
                label="Text Area"
                name="textarea"
                rows={5}
                containerClass={'mb-3'}
                key="textarea"
              />

              <TextInput
                label="Read only"
                type="text"
                name="text1"
                id="text1"
                placeholder="Readonly value"
                containerClass={'mb-3'}
                key="text1"
                readOnly
              />

              <TextInput
                label="Disabled"
                type="text"
                name="text2"
                id="text2"
                placeholder="Disabled value"
                containerClass={'mb-3'}
                key="text2"
                disabled
              />

              <div className="mb-3">
                <Form.Label>Static control</Form.Label>
                <Form.Control
                  plaintext
                  readOnly
                  defaultValue="email@example.com"
                />
              </div>

              <Form.Group className="mb-3">
                <Form.Label htmlFor="text3" className="form-label">
                  Helping text
                </Form.Label>
                <Form.Control
                  type="text"
                  name="text"
                  id="text3"
                  placeholder="Helping text"
                />
                <Form.Text>
                  A block of help text that breaks onto a new line and may
                  extend beyond one line.
                </Form.Text>
              </Form.Group>
            </RHForm>
          </Col>

          <Col lg={6}>
            <RHForm onSubmit={() => {}}>
              <SelectInput
                name="select"
                label="Input Select"
                containerClass="mb-3"
                className="form-select"
                key="select"
              >
                <option>1</option>
                <option>2</option>
                <option>3</option>
                <option>4</option>
                <option>5</option>
              </SelectInput>

              <SelectInput
                name="selectMulti"
                label="Multiple Select"
                multiple
                containerClass="mb-3"
                className="form-select"
                key="selectMulti"
              >
                <option>1</option>
                <option>2</option>
                <option>3</option>
                <option>4</option>
                <option>5</option>
              </SelectInput>

              <FileInput
                label="Default file input"
                type="file"
                name="file"
                containerClass={'mb-3'}
                key="file"
              />

              <DateInput
                label="Date"
                type="date"
                name="date"
                containerClass={'mb-3'}
                key="date"
              />

              <DateInput
                label="Month"
                type="month"
                name="month"
                containerClass={'mb-3'}
                key="month"
              />

              <DateInput
                label="Time"
                type="time"
                name="time"
                containerClass={'mb-3'}
                key="time"
              />

              <DateInput
                label="Week"
                type="week"
                name="week"
                containerClass={'mb-3'}
                key="week"
              />

              <TextInput
                label="Number"
                type="number"
                name="number"
                placeholder="number placeholder"
                containerClass={'mb-3'}
                key="number"
              />

              <ColorInput
                label="Color"
                type="color"
                name="color"
                placeholder="color placeholder"
                className="w-100"
                containerClass={'mb-3'}
                key="color"
              />

              <Form.Group className="mb-0">
                <Form.Label htmlFor="exampleRange" className="form-label">
                  Range
                </Form.Label>
                <Form.Range />
              </Form.Group>
            </RHForm>
          </Col>
        </Row>
      </Card.Body>
    </Card>
  );
};

const FloatingLabels = () => {
  return (
    <Card>
      <Card.Body>
        <h4 className="header-title">Floating labels</h4>
        <p className="text-muted">
          Wrap a <code>&lt;Form.Control&gt;</code> element in
          <code>&lt;FloatingLabel&gt;</code>
          to enable floating labels with Bootstrap’s textual form fields. A
          <code>placeholder</code>
          is required on each <code>&lt;Form.Control&gt;</code> as our method of
          CSS-only floating labels uses the <code>:placeholder-shown</code>{' '}
          pseudo-element.
        </p>

        <Row>
          <Col lg={6}>
            <h5 className="mb-3">Example</h5>
            <FloatingLabel
              controlId="floatingInput"
              label="Email address"
              className="mb-3"
            >
              <Form.Control type="email" placeholder="name@example.com" />
            </FloatingLabel>
            <FloatingLabel controlId="floatingPassword" label="Password">
              <Form.Control type="password" placeholder="Password" />
            </FloatingLabel>

            <h5 className="mb-3 mt-4">Textareas</h5>
            <FloatingLabel controlId="floatingTextarea2" label="Comments">
              <Form.Control
                as="textarea"
                placeholder="Leave a comment here"
                style={{ height: '100px' }}
              />
            </FloatingLabel>
          </Col>

          <Col lg={6}>
            <h5 className="mb-3">Selects</h5>
            <FloatingLabel
              controlId="floatingSelect"
              label="Works with selects"
            >
              <Form.Select aria-label="Floating label select example">
                <option>Open this select menu</option>
                <option value="1">One</option>
                <option value="2">Two</option>
                <option value="3">Three</option>
              </Form.Select>
            </FloatingLabel>

            <h5 className="mb-3 mt-4">Layout</h5>
            <Row className="g-2">
              <Col md>
                <FloatingLabel
                  controlId="floatingInputGrid"
                  label="Email address"
                >
                  <Form.Control type="email" placeholder="name@example.com" />
                </FloatingLabel>
              </Col>
              <Col md>
                <FloatingLabel
                  controlId="floatingSelectGrid"
                  label="Works with selects"
                >
                  <Form.Select aria-label="Floating label select example">
                    <option>Open this select menu</option>
                    <option value="1">One</option>
                    <option value="2">Two</option>
                    <option value="3">Three</option>
                  </Form.Select>
                </FloatingLabel>
              </Col>
            </Row>
          </Col>
        </Row>
      </Card.Body>
    </Card>
  );
};

const SelectInputDemo = () => {
  return (
    <>
      <h4 className="header-title">Select menu</h4>
      <p className="text-muted">
        Custom <code>&lt;select&gt;</code> menus need only a custom class,
        <code>.custom-select</code> to trigger the custom styles.
      </p>

      <Form.Group>
        <Form.Select
          className="custom-select mt-3"
          aria-label="Default select example"
        >
          <option>1</option>
          <option>2</option>
          <option>3</option>
          <option>4</option>
          <option>5</option>
        </Form.Select>
      </Form.Group>
    </>
  );
};

const Switches = () => {
  return (
    <>
      <h4 className="header-title mt-4">Switches</h4>
      <p className="text-muted">
        A switch has the markup of a custom checkbox but uses the
        <code>.custom-switch</code> class to render a toggle switch. Switches
        also support the <code>disabled</code> attribute.
      </p>

      <Form>
        <Form.Check
          type="switch"
          id="custom-switch"
          label="Toggle this switch element"
        />
        <Form.Check
          disabled
          type="switch"
          label="Disabled switch element"
          id="disabled-custom-switch"
          className="mt-1"
        />
      </Form>
    </>
  );
};

const CustomCheckboxes = () => {
  return (
    <Form.Group>
      <Form.Label className="form-label" htmlFor="exampleCheckbox">
        Checkboxes
      </Form.Label>
      <div>
        <Form.Check
          type="checkbox"
          id="default-checkbox"
          label="Check this custom checkbox"
        />
        <Form.Check
          type="checkbox"
          id="default-checkbox"
          label="But not this disabled one"
          disabled
        />
      </div>
    </Form.Group>
  );
};

const InlineCustomCheckboxes = () => {
  return (
    <Form.Group className="mt-2">
      <Form.Label className="form-check-label" htmlFor="exampleCheckbox">
        Inline
      </Form.Label>
      <div>
        <Form.Check
          type="checkbox"
          id="inline-checkbox"
          label="An inline custom input"
        />
        <Form.Check
          type="checkbox"
          id="inline-checkbox"
          label="and another one"
        />
      </div>
    </Form.Group>
  );
};

const CustomRadios = () => {
  return (
    <Form.Group className="mt-2">
      <Form.Label className="form-label" htmlFor="exampleCheckbox">
        Radios
      </Form.Label>
      <div>
        <Form.Check
          type="radio"
          id="default-radio1"
          label="Check this custom checkbox"
        />
        <Form.Check
          type="radio"
          id="default-radio1"
          label="But not this disabled one"
          disabled
        />
      </div>
    </Form.Group>
  );
};

const InputSizes = () => {
  return (
    <Card>
      <Card.Body>
        <h4 className="header-title">Input Sizes</h4>
        <p className="text-muted">
          Use size on <code>&lt;FormControl&gt;</code> and{' '}
          <code>&lt;FormLabel&gt;</code>
          to change the size of inputs and labels respectively.
        </p>

        <Form>
          <Form.Group className="mb-3">
            <Form.Label htmlFor="small">Small</Form.Label>
            <Form.Control
              type="text"
              name="small"
              id="small"
              placeholder="Small"
              size="sm"
            />
          </Form.Group>

          <Form.Group className="mb-3">
            <Form.Label htmlFor="Normal">Normal</Form.Label>
            <Form.Control
              type="text"
              name="Normal"
              id="Normal"
              placeholder="Normal"
            />
          </Form.Group>

          <Form.Group className="mb-3">
            <Form.Label htmlFor="Large">Large</Form.Label>
            <Form.Control
              type="text"
              name="Large"
              id="Large"
              placeholder="Large"
              size="lg"
            />
          </Form.Group>

          <Form.Group className="mb-0">
            <Form.Label htmlFor="grid">Grid Sizes</Form.Label>
            <Row>
              <Col sm={4}>
                <Form.Control
                  type="text"
                  name="grid"
                  id="grid"
                  placeholder=".col-sm-4"
                />
              </Col>
            </Row>
          </Form.Group>
        </Form>
      </Card.Body>
    </Card>
  );
};

const InputGroups = () => {
  return (
    <Card>
      <Card.Body>
        <h4 className="header-title">Input Group</h4>
        <p className="text-muted">
          Easily extend form conrols by adding text, buttons, or button groups
          on either side of textual inputs, custom selects, and custom file
          inputs
        </p>

        <Form>
          <Form.Group>
            <Form.Label htmlFor="Static">Static</Form.Label>
            <InputGroup className="mb-3">
              <InputGroup.Text id="basic-addon1">@</InputGroup.Text>
              <Form.Control
                placeholder="Username"
                aria-label="Username"
                aria-describedby="basic-addon1"
              />
            </InputGroup>
          </Form.Group>

          <Form.Group>
            <Form.Label htmlFor="Dropdown">Dropdown</Form.Label>
            <InputGroup className="mb-3">
              <DropdownButton
                variant="primary"
                title="Dropdown"
                id="input-group-dropdown-1"
              >
                <Dropdown.Item href="">Action</Dropdown.Item>
                <Dropdown.Item href="">Another action</Dropdown.Item>
                <Dropdown.Item href="">Something else here</Dropdown.Item>
                <Dropdown.Divider />
                <Dropdown.Item href="">Separated link</Dropdown.Item>
              </DropdownButton>
              <Form.Control aria-label="Text input with dropdown button" />
            </InputGroup>
          </Form.Group>

          <Form.Group>
            <Form.Label htmlFor="Button">Button</Form.Label>
            <InputGroup className="mb-3">
              <Form.Control
                placeholder="Recipient's username"
                aria-label="Recipient's username"
                aria-describedby="basic-addon2"
              />
              <Button variant="dark" id="button-addon2">
                Button
              </Button>
            </InputGroup>
          </Form.Group>

          <Row className="g-2">
            <Col sm={6}>
              <Form.Group>
                <Form.Label htmlFor="file">File input</Form.Label>
                <Form.Control type="file" />
              </Form.Group>
            </Col>
            <Col sm={6}>
              <Form.Group>
                <Form.Label htmlFor="formFileMultiple01">
                  Multiple input
                </Form.Label>
                <Form.Control type="file" multiple />
              </Form.Group>
            </Col>
          </Row>
        </Form>
      </Card.Body>
    </Card>
  );
};

const DefaultForm = () => {
  return (
    <Card>
      <Card.Body>
        <h4 className="mb-3 header-title">Basic Example</h4>

        <Form>
          <Form.Group className="mb-3">
            <Form.Label htmlFor="exampleEmail2">Email</Form.Label>
            <Form.Control
              type="email"
              name="email"
              id="exampleEmail2"
              placeholder="Enter your email"
            />
            <Form.Text>
              We'll never share your email with anyone else.
            </Form.Text>
          </Form.Group>

          <Form.Group className="mb-3">
            <Form.Label htmlFor="examplePassword2">Password</Form.Label>
            <Form.Control
              type="password"
              name="password"
              id="examplePassword2"
              placeholder="password placeholder"
              defaultValue="12345"
            />
          </Form.Group>

          <Form.Group className="mb-3" id="formGridCheckbox">
            <Form.Check type="checkbox" label="Check me out" />
          </Form.Group>

          <Button variant="primary" type="submit">
            Submit
          </Button>
        </Form>
      </Card.Body>
    </Card>
  );
};

const HorizontalForm = () => {
  return (
    <Card>
      <Card.Body>
        <h4 className="mb-3 header-title">Horizontal Form</h4>
        <Form>
          <Form.Group as={Row} className="mb-3">
            <Form.Label htmlFor="exampleEmail3" column sm={3}>
              Email
            </Form.Label>
            <Col sm={9}>
              <Form.Control
                type="email"
                name="email"
                id="exampleEmail3"
                placeholder="Enter your email"
              />
              <Form.Text>
                We'll never share your email with anyone else.
              </Form.Text>
            </Col>
          </Form.Group>

          <Form.Group as={Row} className="mb-3">
            <Form.Label htmlFor="examplePassword3" column sm={3}>
              Password
            </Form.Label>
            <Col sm={9}>
              <Form.Control
                type="password"
                name="password"
                id="examplePassword3"
                placeholder="password placeholder"
                defaultValue="12345"
              />
            </Col>
          </Form.Group>

          <Form.Group as={Row} className="mb-3">
            <Form.Label htmlFor="examplePassword4" column sm={3}>
              Re-Password
            </Form.Label>
            <Col sm={9}>
              <Form.Control
                type="password"
                name="password"
                id="examplePassword4"
                placeholder="password placeholder"
                defaultValue="12345"
              />
            </Col>
          </Form.Group>

          <Form.Group as={Row} className="mb-3" controlId="formHorizontalCheck">
            <Col sm={{ span: 9, offset: 3 }}>
              <Form.Check label="Remember me" />
            </Col>
          </Form.Group>

          <Form.Group as={Row} className="mb-0" controlId="formHorizontalCheck">
            <Col sm={{ span: 9, offset: 3 }}>
              <Button variant="primary" type="submit">
                Sign in
              </Button>
            </Col>
          </Form.Group>
        </Form>
      </Card.Body>
    </Card>
  );
};

const InlineForm = () => {
  return (
    <Card>
      <Card.Body>
        <h4 className="mb-3 header-title">Inline Form Example</h4>
        <RHForm
          onSubmit={() => {}}
          className="row row-cols-lg-auto g-3 align-items-center"
          defaultValues={{ password: '1234556' }}
        >
          <Col>
            <TextInput
              type="email"
              name="email"
              id={'exampleEmail4'}
              containerClass="mb-2 me-sm-2 mb-sm-0"
              placeholder="email@example.com"
              bsPrefix="form-control-plaintext"
              readOnly
            />
          </Col>

          <Col>
            <PasswordInput
              type="password"
              name="password"
              id="examplePassword5"
              placeholder="password placeholder"
            />
          </Col>

          <Col>
            <Button color="primary" type="submit">
              Confirm identity
            </Button>
          </Col>
        </RHForm>

        <h6 className="font-13 mt-3">Auto-sizing</h6>

        <Form>
          <Row className="align-items-center">
            <Col xs="auto">
              <Form.Label htmlFor="inlineFormInput" visuallyHidden>
                Name
              </Form.Label>
              <Form.Control
                className="mb-2"
                id="inlineFormInput"
                placeholder="Jane Doe"
              />
            </Col>
            <Col xs="auto">
              <Form.Label htmlFor="inlineFormInputGroup" visuallyHidden>
                Username
              </Form.Label>
              <InputGroup className="mb-2">
                <InputGroup.Text>@</InputGroup.Text>
                <Form.Control
                  id="inlineFormInputGroup"
                  placeholder="Username"
                />
              </InputGroup>
            </Col>
            <Col xs="auto">
              <Form.Check
                type="checkbox"
                id="autoSizingCheck"
                className="mb-2"
                label="Remember me"
              />
            </Col>
            <Col xs="auto">
              <Button type="submit" className="mb-2">
                Submit
              </Button>
            </Col>
          </Row>
        </Form>
      </Card.Body>
    </Card>
  );
};

const HorizontalFormLabelSizing = () => {
  return (
    <Card>
      <Card.Body>
        <h4 className="header-title">Horizontal form label sizing</h4>
        <p className="text-muted font-14">
          You can size the <code>&lt;FormLabel&gt;</code> using the column prop
          as shown.
        </p>

        <Row>
          <Form.Label column="lg" lg={2}>
            Large Text
          </Form.Label>
          <Col>
            <Form.Control size="lg" type="text" placeholder="Large text" />
          </Col>
        </Row>
        <br />
        <Row>
          <Form.Label column lg={2}>
            Normal Text
          </Form.Label>
          <Col>
            <Form.Control type="text" placeholder="Normal text" />
          </Col>
        </Row>
        <br />
        <Row>
          <Form.Label column="sm" lg={2}>
            Small Text
          </Form.Label>
          <Col>
            <Form.Control size="sm" type="text" placeholder="Small text" />
          </Col>
        </Row>
      </Card.Body>
    </Card>
  );
};

const FormGrid = () => {
  return (
    <Card>
      <Card.Body>
        <h4 className="header-title">Form Grid</h4>

        <Form>
          <Row className="mb-3">
            <Form.Group as={Col} controlId="formGridEmail">
              <Form.Label>Email</Form.Label>
              <Form.Control type="email" placeholder="Enter email" />
            </Form.Group>

            <Form.Group as={Col} controlId="formGridPassword">
              <Form.Label>Password</Form.Label>
              <Form.Control type="password" placeholder="Password" />
            </Form.Group>
          </Row>

          <Form.Group className="mb-3" controlId="formGridAddress1">
            <Form.Label>Address</Form.Label>
            <Form.Control placeholder="1234 Main St" />
          </Form.Group>

          <Form.Group className="mb-3" controlId="formGridAddress2">
            <Form.Label>Address 2</Form.Label>
            <Form.Control placeholder="Apartment, studio, or floor" />
          </Form.Group>

          <Row className="mb-3">
            <Form.Group as={Col} controlId="formGridCity">
              <Form.Label>City</Form.Label>
              <Form.Control />
            </Form.Group>

            <Form.Group as={Col} controlId="formGridState">
              <Form.Label>State</Form.Label>
              <Form.Select defaultValue="Choose...">
                <option>Choose...</option>
                <option>...</option>
              </Form.Select>
            </Form.Group>

            <Form.Group as={Col} controlId="formGridZip">
              <Form.Label>Zip</Form.Label>
              <Form.Control />
            </Form.Group>
          </Row>

          <Form.Group className="mb-3" id="formGridCheckbox2">
            <Form.Check type="checkbox" label="Check me out" />
          </Form.Group>

          <Button variant="primary" type="submit">
            Submit
          </Button>
        </Form>
      </Card.Body>
    </Card>
  );
};

const FormBasic = () => {
  return (
    <PageLayout title="Form Basic">
      <Row>
        <Col>
          <BasicInputElements />
        </Col>
      </Row>

      <Row>
        <Col>
          <FloatingLabels />
        </Col>
      </Row>

      <Row>
        <Col>
          <Card>
            <Card.Body>
              <Row>
                <Col lg={6}>
                  <SelectInputDemo />
                  <Switches />
                </Col>

                <Col lg={6}>
                  <h4 className="header-title mt-5 mt-sm-0">
                    Checkboxes and radios
                  </h4>
                  <div className="mt-3">
                    <CustomCheckboxes />
                    <CustomRadios />
                    <InlineCustomCheckboxes />
                  </div>
                </Col>
              </Row>
            </Card.Body>
          </Card>
        </Col>
      </Row>

      <Row>
        <Col lg={6}>
          <InputSizes />
        </Col>

        <Col lg={6}>
          <InputGroups />
        </Col>
      </Row>

      <Row>
        <Col lg={6}>
          <DefaultForm />
        </Col>

        <Col lg={6}>
          <HorizontalForm />
        </Col>
      </Row>

      <Row>
        <Col>
          <InlineForm />
        </Col>
      </Row>

      <Row>
        <Col>
          <HorizontalFormLabelSizing />
        </Col>
      </Row>

      <Row>
        <Col>
          <FormGrid />
        </Col>
      </Row>
    </PageLayout>
  );
};

export default FormBasic;
