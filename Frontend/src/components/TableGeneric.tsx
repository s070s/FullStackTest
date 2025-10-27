import React, { useMemo } from "react";
import { API_BASE_URL } from "../utils/api/api";

// Table column definition
type TableColumn = {
  key: string;
  label: string;
};

// Props for generic table component
type TableGenericProps<T extends object> = {
  data: T[];
  columns?: TableColumn[];
  onSort?: (col: string) => void;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
  selectable?: boolean;
  selectedRowId?: number | null;
  onRowSelect?: (row: T | null) => void;
  renderCell?: (col: string, row: T) => React.ReactNode;
};

function TableGeneric<T extends { id?: number }>({
  data,
  columns: customColumns,
  onSort,
  sortBy,
  sortOrder,
  selectable = false,
  selectedRowId,
  onRowSelect,
  renderCell,
}: TableGenericProps<T>) {
  // Show message if no data
  if (!data || data.length === 0 || !data[0]) return <div>No data available.</div>;

  // Determine columns: use custom or infer from data
  const columns = useMemo(() => {
    if (customColumns && customColumns.length > 0) {
      const hasActions = !!(renderCell && data.length > 0 && renderCell("__actions", data[0]) !== undefined);
      return hasActions ? [...customColumns, { key: "__actions", label: "" }] : customColumns;
    }
    const base = Object.keys(data[0]).map(key => ({ key, label: key }));
    const hasActions = !!(renderCell && data.length > 0 && renderCell("__actions", data[0]) !== undefined);
    return hasActions ? [...base, { key: "__actions", label: "" }] : base;
  }, [customColumns, data, renderCell]);

  // Render table rows
  const rows = useMemo(() => {
    return data.map((row, idx) => {
      const isSelected = selectable && selectedRowId === (row as any).id;
      return (
        <tr
          key={(row as any).id ?? idx}
          style={isSelected ? { backgroundColor: "#e0f7fa" } : {}}
          onClick={() => {
            if (selectable && onRowSelect) {
              onRowSelect(row);
            }
          }}
        >
          {columns.map((col) => (
            <td key={col.key}>
              {renderCell && renderCell(col.key, row) !== undefined
                ? renderCell(col.key, row)
                // Special rendering for profile photo
                : col.key === "profilePhotoUrl" ? (
                  <img
                    src={
                      (row as any)[col.key]
                        ? `${API_BASE_URL}${(row as any)[col.key]}?t=${(row as any).id}`
                        : "/default-avatar.png"
                    }
                    alt="Profile"
                    width={40}
                    height={40}
                    style={{ borderRadius: "50%", objectFit: "cover" }}
                    onError={e => {
                      (e.currentTarget as HTMLImageElement).src = "/default-avatar.png";
                    }}
                  />
                ) : (
                  String((row as any)[col.key])
                )}
            </td>
          ))}
        </tr>
      );
    });
  }, [data, columns, renderCell, selectable, selectedRowId, onRowSelect]);

  return (
    <table>
      <thead>
        <tr>
          {columns.map((col) => (
            <th key={col.key} onClick={() => onSort?.(col.key)}>
              {col.key !== "__actions" ? col.label : ""}
              {sortBy === col.key && col.key !== "__actions" && (
                sortOrder === "asc" ? (
                  <i className="fas fa-arrow-up" aria-label="Sort ascending"></i>
                ) : (
                  <i className="fas fa-arrow-down" aria-label="Sort descending"></i>
                )
              )}
            </th>
          ))}
        </tr>
      </thead>
      <tbody>
        {rows}
      </tbody>
    </table>
  );
}

// Memoize for performance
export default React.memo(TableGeneric) as unknown as typeof TableGeneric;