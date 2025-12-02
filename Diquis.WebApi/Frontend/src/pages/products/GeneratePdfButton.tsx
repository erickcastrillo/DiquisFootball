import React, { useState } from 'react';
import { observer } from 'mobx-react-lite';
import { useStore } from "stores/store";
import { Button} from "react-bootstrap";
import { Product } from 'lib/types';

interface Props {
  product: Product;
}

const ProductExportComponent = observer(({ product }: Props) => {
  const { productsStore } = useStore();
  const [exporting, setExporting] = useState(false);

  const handleExport = async () => {
    setExporting(true);
    await productsStore.generatePdf(product.id);
    setExporting(false);
  };

  return (
    <Button
      variant="success"
      className=" mx-2 px-3 py-1 text-sm bg-green-500 text-white rounded-full hover:bg-green-600 disabled:opacity-50 disabled:cursor-not-allowed rounded-pill"
      onClick={handleExport}
      disabled={exporting}
      
    >
      {exporting ? "Exporting..." : "PDF"}
    </Button>
  );
});

export default ProductExportComponent;