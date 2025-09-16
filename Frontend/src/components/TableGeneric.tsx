import { API_BASE_URL } from "../utils/api/api"; // Import your API base URL

type TableGenericProps<T extends object> = {
  data: T[];
  onSort?: (col: string) => void;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
};

function TableGeneric<T extends object>({
  data,
  onSort,
  sortBy,
  sortOrder,
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
              {sortBy === col && (sortOrder === "asc" ? " ▲" : " ▼")}
            </th>
          ))}
        </tr>
      </thead>
      <tbody>
        {data.map((row, idx) => (
          <tr key={idx}>
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
        ))}
      </tbody>
    </table>
  );
}

export default TableGeneric;