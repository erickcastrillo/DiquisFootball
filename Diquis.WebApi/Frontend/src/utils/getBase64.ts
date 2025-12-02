const getBase64 = (file: File) => {
    return new Promise((res, rej) => {
        const reader = new FileReader();
        reader.onload = e => res(e?.target?.result); //define condition if loaded correctly
        reader.onerror = e => rej(e); //define condition if loaded incorrectly
        reader.readAsDataURL(file); //the actual operation
    });
    
};

export default getBase64