// TypeScript/React coding patterns for GitHub Copilot to learn from
// This file demonstrates project conventions for frontend components

// =============================================================================
// TYPESCRIPT/REACT PATTERNS FOR COPILOT
// =============================================================================

import { Link, useForm } from "@inertiajs/react";
import React, { useEffect, useState } from "react";

// Pattern 1: Type Definitions
interface Academy {
  id: string;
  name: string;
  slug: string;
}

interface Example {
  id: string;
  name: string;
  description: string;
  slug: string;
  academy_id: string;
  created_at: string;
  updated_at: string;
}

interface ExampleIndexProps {
  examples: Example[];
  academy: Academy;
}

interface ExampleFormProps {
  example?: Example;
  academy: Academy;
}

// Pattern 2: Index Component Structure
export default function ExampleIndex({ examples, academy }: ExampleIndexProps) {
  const [searchTerm, setSearchTerm] = useState("");

  const filteredExamples = examples.filter((example) =>
    example.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="container">
      <div className="header-section">
        <h1>Examples for {academy.name}</h1>
        <Link
          href={`/app/${academy.slug}/examples/new`}
          className="btn btn-primary"
        >
          Create New Example
        </Link>
      </div>

      <div className="search-section">
        <input
          type="text"
          placeholder="Search examples..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="form-control"
        />
      </div>

      <div className="examples-grid">
        {filteredExamples.map((example) => (
          <div key={example.id} className="example-card">
            <h3>{example.name}</h3>
            <p>{example.description}</p>
            <div className="card-actions">
              <Link
                href={`/app/${academy.slug}/examples/${example.id}`}
                className="btn btn-outline-primary"
              >
                View
              </Link>
              <Link
                href={`/app/${academy.slug}/examples/${example.id}/edit`}
                className="btn btn-outline-secondary"
              >
                Edit
              </Link>
            </div>
          </div>
        ))}
      </div>

      {filteredExamples.length === 0 && (
        <div className="empty-state">
          <h3>No examples found</h3>
          <p>Try adjusting your search or create a new example.</p>
        </div>
      )}
    </div>
  );
}

// Pattern 3: Show Component Structure
export function ExampleShow({
  example,
  academy,
}: {
  example: Example;
  academy: Academy;
}) {
  return (
    <div className="container">
      <div className="header-section">
        <nav className="breadcrumb">
          <Link href={`/app/${academy.slug}/examples`}>Examples</Link>
          <span className="breadcrumb-separator">/</span>
          <span>{example.name}</span>
        </nav>
      </div>

      <div className="example-detail">
        <h1>{example.name}</h1>
        <div className="example-meta">
          <span>
            Created: {new Date(example.created_at).toLocaleDateString()}
          </span>
          <span>
            Updated: {new Date(example.updated_at).toLocaleDateString()}
          </span>
        </div>
        <div className="example-description">
          <p>{example.description}</p>
        </div>
      </div>

      <div className="action-section">
        <Link
          href={`/app/${academy.slug}/examples/${example.id}/edit`}
          className="btn btn-primary"
        >
          Edit Example
        </Link>
        <Link
          href={`/app/${academy.slug}/examples`}
          className="btn btn-secondary"
        >
          Back to Examples
        </Link>
      </div>
    </div>
  );
}

// Pattern 4: Form Component Structure with Inertia.js
export function ExampleForm({ example, academy }: ExampleFormProps) {
  const { data, setData, post, put, processing, errors } = useForm({
    name: example?.name || "",
    description: example?.description || "",
  });

  const isEditing = !!example;

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (isEditing) {
      put(`/app/${academy.slug}/examples/${example.id}`);
    } else {
      post(`/app/${academy.slug}/examples`);
    }
  };

  return (
    <div className="container">
      <div className="header-section">
        <h1>{isEditing ? "Edit Example" : "Create New Example"}</h1>
        <p>Academy: {academy.name}</p>
      </div>

      <form onSubmit={handleSubmit} className="example-form">
        <div className="form-group">
          <label htmlFor="name" className="form-label">
            Name *
          </label>
          <input
            type="text"
            id="name"
            value={data.name}
            onChange={(e) => setData("name", e.target.value)}
            className={`form-control ${errors.name ? "is-invalid" : ""}`}
            placeholder="Enter example name"
            required
          />
          {errors.name && <div className="invalid-feedback">{errors.name}</div>}
        </div>

        <div className="form-group">
          <label htmlFor="description" className="form-label">
            Description
          </label>
          <textarea
            id="description"
            value={data.description}
            onChange={(e) => setData("description", e.target.value)}
            className={`form-control ${errors.description ? "is-invalid" : ""}`}
            rows={4}
            placeholder="Enter description (optional)"
          />
          {errors.description && (
            <div className="invalid-feedback">{errors.description}</div>
          )}
        </div>

        <div className="form-actions">
          <button
            type="submit"
            disabled={processing}
            className="btn btn-primary"
          >
            {processing
              ? "Saving..."
              : isEditing
              ? "Update Example"
              : "Create Example"}
          </button>
          <Link
            href={`/app/${academy.slug}/examples`}
            className="btn btn-secondary"
          >
            Cancel
          </Link>
        </div>
      </form>
    </div>
  );
}

// Pattern 5: Custom Hook Pattern
export function useExampleSearch(examples: Example[]) {
  const [searchTerm, setSearchTerm] = useState("");
  const [filteredExamples, setFilteredExamples] = useState(examples);

  useEffect(() => {
    const filtered = examples.filter(
      (example) =>
        example.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        example.description.toLowerCase().includes(searchTerm.toLowerCase())
    );
    setFilteredExamples(filtered);
  }, [examples, searchTerm]);

  return {
    searchTerm,
    setSearchTerm,
    filteredExamples,
  };
}

// Pattern 6: Error Handling Component
interface ErrorDisplayProps {
  errors: Record<string, string[]>;
}

export function ErrorDisplay({ errors }: ErrorDisplayProps) {
  const errorMessages = Object.values(errors).flat();

  if (errorMessages.length === 0) {
    return null;
  }

  return (
    <div className="alert alert-danger">
      <h4>Please fix the following errors:</h4>
      <ul>
        {errorMessages.map((error, index) => (
          <li key={index}>{error}</li>
        ))}
      </ul>
    </div>
  );
}

// Pattern 7: Loading State Component
interface LoadingSpinnerProps {
  message?: string;
}

export function LoadingSpinner({
  message = "Loading...",
}: LoadingSpinnerProps) {
  return (
    <div className="loading-container">
      <div className="spinner-border" role="status">
        <span className="visually-hidden">{message}</span>
      </div>
      <p>{message}</p>
    </div>
  );
}

// Pattern 8: Utility Functions
export const formatDate = (dateString: string): string => {
  return new Date(dateString).toLocaleDateString("en-US", {
    year: "numeric",
    month: "long",
    day: "numeric",
  });
};

export const truncateText = (text: string, maxLength: number): string => {
  if (text.length <= maxLength) return text;
  return text.substring(0, maxLength) + "...";
};

export const generateSlug = (text: string): string => {
  return text
    .toLowerCase()
    .replace(/[^a-z0-9 -]/g, "")
    .replace(/\s+/g, "-")
    .replace(/-+/g, "-")
    .trim();
};
