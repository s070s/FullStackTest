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
            <th
              key={col}
              style={{ cursor: onSort ? "pointer" : "default" }}
              onClick={onSort ? () => onSort(col) : undefined}
            >
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
              <td key={col}>{String(row[col as keyof typeof row])}</td>
            ))}
          </tr>
        ))}
      </tbody>
    </table>
  );
}

export default TableGeneric;