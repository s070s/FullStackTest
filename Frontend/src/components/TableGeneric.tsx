import { API_BASE_URL } from "../utils/api/api"; // Import your API base URL

type TableGenericProps<T extends object> = {
  data: T[];
  onSort?: (col: string) => void;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
  selectable?: boolean;
  selectedRowId?: number | null;
  onRowSelect?: (row: T | null) => void;
};

function TableGeneric<T extends { id?: number }>({
  data,
  onSort,
  sortBy,
  sortOrder,
  selectable = false,
  selectedRowId,
  onRowSelect,
}: TableGenericProps<T>) {
  console.log("TableGeneric data:", data);

  if (!data || data.length === 0 || !data[0]) return <div>No data available.</div>;

  const columns = Object.keys(data[0]);

  return (
    <table>
      <thead>
        <tr>
          {columns.map((col) => (
            <th key={col} onClick={() => onSort?.(col)}>
              {col}
              {sortBy === col && (
                sortOrder === "asc" ? (
                  <i className="fas fa-arrow-up"  aria-label="Sort ascending"></i>
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
                  {col === "profilePhotoUrl" && (row as any)[col] ? (
                    <img
                      src={`${API_BASE_URL}${(row as any)[col]}?t=${(row as any).id}`}
                      alt="Profile"
                      width={40}
                      height={40}
                      style={{ borderRadius: "50%", objectFit: "cover" }}
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