# Task Context
Create the frontend UI for signing documents. This includes a `SignaturePad` component using a canvas library and a page to view the document content.

# Core References
- **Plan:** [18 - Implementation_Plan_DigitalSignatures.md](./18%20-%20Implementation_Plan_DigitalSignatures.md)

# Step-by-Step Instructions
1.  **Install Dependencies:**
    *   `npm install react-signature-canvas`.
2.  **Create `SignaturePad.tsx`:**
    *   Path: `src/features/signatures/components/SignaturePad.tsx`.
    *   Render canvas.
    *   Methods: `clear()`, `submitSignature()` (returns Base64).
3.  **Create `SignDocumentPage.tsx`:**
    *   Path: `src/pages/signatures/SignDocumentPage.tsx`.
    *   Fetch document content (HTML/Markdown) and render.
    *   Embed `SignaturePad` at the bottom.
    *   On submit, call API to finalize signature.

# Acceptance Criteria
- [ ] Dependencies installed.
- [ ] `SignaturePad` captures signature.
- [ ] `SignDocumentPage` displays content and handles submission.
