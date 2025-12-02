export type CheckPasswordValidationOptionsType = 'length' | 'lowerCase' | 'upperCase' | 'number' | 'specialCharacter';

export type CheckPasswordOptionsType = {
  checkFor: CheckPasswordValidationOptionsType[],
}

const validationOptions = {
  length:  new RegExp('(?=.{6,})'),
  lowerCase: new RegExp('(?=.*[a-z])'),
  upperCase: new RegExp('(?=.*[A-Z])'),
  number:  new RegExp('(?=.*[0-9])'),
  specialCharacter: new RegExp('(?=.*[^A-Za-z0-9])')
}

export interface CheckPasswordResult {
    strength: number;
    passedFor: CheckPasswordValidationOptionsType[];
}

const checkPasswordStrength = (password: string, options: CheckPasswordOptionsType): CheckPasswordResult => {
    let calculatedStrength = 0;
    const passedFor: CheckPasswordValidationOptionsType[] = [];
    const raiseStrengthBy = (100 / options.checkFor.length).toFixed(2);
    options.checkFor.forEach(option => {
        if (validationOptions[option]?.test(password)) {
            passedFor.push(option)
            calculatedStrength += parseFloat(raiseStrengthBy);
        }
    })
    return {strength: calculatedStrength, passedFor};
};

export default checkPasswordStrength