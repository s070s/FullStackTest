type TableGenericProps<T extends object> = {
  data: T[];
};

function TableGeneric<T extends object>({ data }: TableGenericProps<T>) {
  if (data.length === 0) return <div>No data available.</div>;

  const columns = Object.keys(data[0]);

  return (
    <table>
      <thead>
        <tr>
          {columns.map((col) => (
            <th key={col}>{col}</th>
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