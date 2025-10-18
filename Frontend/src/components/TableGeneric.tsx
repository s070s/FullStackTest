import { API_BASE_URL } from "../utils/api/api"; // Import your API base URL

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

  let columns = Object.keys(data[0]);
  if (renderCell && data.length > 0 && renderCell("__actions", data[0]) !== undefined) {
    columns = [...columns, "__actions"];
  }

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
        {data.map((row, idx) => {
          const isSelected = selectable && selectedRowId === (row as any).id;
          return (
            <tr
              key={idx}
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
        })}
      </tbody>
    </table>
  );
}

export default TableGeneric;