const getInitials = (firstName: string, lastName: string) => {
  const firstInitial = firstName?.[0] || '';
  const lastInitial = lastName?.[0] || '';
  return firstInitial + lastInitial;
};

export default getInitials;
