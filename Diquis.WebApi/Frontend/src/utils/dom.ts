const changeBodyAttribute = (attribute: string, value: string): void => {
  if (document.body) document.body.setAttribute(attribute, value);
};

const changeHtmlAttribute = (attribute: string, value: string): void => {
  const htmlElement = document.querySelector('html');
  if (htmlElement) htmlElement.setAttribute(attribute, value);
};

export { changeBodyAttribute, changeHtmlAttribute };
