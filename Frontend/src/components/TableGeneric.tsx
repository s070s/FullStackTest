import React, { useMemo } from "react";
import { API_BASE_URL } from "../utils/api/api";

/**
 * Generic, memoized table component.
 * - Displays rows for an array of objects (T). Uses Object.keys of the first item for columns.
 * - Optional renderCell(col, row) lets callers override cell rendering (used for action cells).
 * - Memoizes columns and row JSX to avoid unnecessary re-renders when props are stable.
 * - Handles profile photo rendering with a default avatar and an onError fallback.
 * - Supports selectable rows (highlights selectedRowId and calls onRowSelect).
 * - Emits onSort when a column header is clicked (skip __actions column).
 */
type TableGenericProps<T extends object> = {
  data: T[];
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
  onSort,
  sortBy,
  sortOrder,
  selectable = false,
  selectedRowId,
  onRowSelect,
  renderCell,
}: TableGenericProps<T>) {
  if (!data || data.length === 0 || !data[0]) return <div>No data available.</div>;

  //memoize columns  
  const columns = useMemo(()=>{
    const base = Object.keys(data[0]);
    const hasActions = !!(renderCell && data.length > 0 && renderCell("__actions", data[0]) !== undefined);
    return hasActions ? [...base, "__actions"] : base;
  }, [data, renderCell]);

//memoize rows
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
            <td key={col}>
              {renderCell && renderCell(col, row) !== undefined
                ? renderCell(col, row)
                : col === "profilePhotoUrl" ? (
                  <img
                    src={
                      (row as any)[col]
                        ? `${API_BASE_URL}${(row as any)[col]}?t=${(row as any).id}`
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
                  String((row as any)[col])
                )}
            </td>
          ))}
        </tr>
      );
    });
  // include deps that, when changed, should recompute rows
  }, [data, columns, renderCell, selectable, selectedRowId, onRowSelect]);

  return (
    <table>
      <thead>
        <tr>
          {columns.map((col) => (
            <th key={col} onClick={() => onSort?.(col)}>
              {col !== "__actions" ? col : ""}
              {sortBy === col && col !== "__actions" && (
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

// memoize component to prevent unnecessary re-renders when props are shallowly equal
export default React.memo(TableGeneric) as unknown as typeof TableGeneric;