import React, { useState } from "react";

const TableStatusPage: React.FC = () => {
    const [tables, setTables] = useState([
        { id: 1, status: "empty", customer: null },
        { id: 2, status: "occupied", customer: "John Doe" },
        { id: 3, status: "empty", customer: null },
        { id: 4, status: "occupied", customer: "Jane Smith" },
        { id: 5, status: "empty", customer: null },
        { id: 6, status: "empty", customer: null },
    ]);

    const handleTableClick = (id: number) => {
        setTables((prevTables) =>
            prevTables.map((table) =>
                table.id === id
                    ? { ...table, status: table.status === "empty" ? "occupied" : "empty" }
                    : table
            )
        );
    };

    return (
        <div className="min-h-screen bg-gray-50 p-8">
            <h1 className="text-4xl font-semibold text-center mb-8 text-blue-600">Table Management</h1>

            <div className="flex flex-wrap justify-center gap-6">
                {tables.map((table) => (
                    <div
                        key={table.id}
                        onClick={() => handleTableClick(table.id)}
                        className={`w-64 p-6 text-center rounded-lg shadow-lg cursor-pointer transition-all duration-300 transform ${table.status === "empty"
                                ? "bg-green-200 hover:bg-green-300 text-green-800"
                                : "bg-red-200 hover:bg-red-300 text-red-800"
                            } hover:scale-105`}
                    >
                        <div className="flex justify-center mb-4">
                            <span className="text-5xl">
                                {table.status === "empty" ? "🪑" : "👥"}
                            </span>
                        </div>
                        <p className="font-semibold text-lg">Table {table.id}</p>
                        <p className="text-sm mb-2">
                            {table.status === "empty" ? "Available" : `Occupied by ${table.customer}`}
                        </p>
                        <div
                            className={`inline-block text-xs py-1 px-3 rounded-full ${table.status === "empty"
                                    ? "bg-green-100 text-green-800"
                                    : "bg-red-100 text-red-800"
                                }`}
                        >
                            {table.status === "empty" ? "Available" : "Occupied"}
                        </div>
                    </div>
                ))}
            </div>

            {/* Optional Modal for Detailed Table Information */}
            {/* Add this if you want to show detailed information when a table is clicked */}
        </div>
    );
};

export default TableStatusPage;
