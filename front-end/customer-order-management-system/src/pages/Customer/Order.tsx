import React, { useState } from "react";
import { useNavigate } from "react-router-dom";

const Order: React.FC = () => {
    const [selectedCategory, setSelectedCategory] = useState<string>("All");
    const [modalOpen, setModalOpen] = useState<boolean>(false);
    const [selectedItem, setSelectedItem] = useState<any>(null);
    const [quantity, setQuantity] = useState<number>(1);
    const [cartItems, setCartItems] = useState<any[]>([]); // Store cart items
    const [note, setNote] = useState<string>(""); // Note for each item
    const navigate = useNavigate();

    const categories = [
        { name: "All", count: 56 },
        { name: "Hotpot", count: 14 },
        { name: "Kimchi Hotpot", count: 7 },
        { name: "Toppings", count: 29 },
        { name: "Beverages", count: 6 },
    ];

    const items = [
        {
            name: "Combo for 1 person",
            price: 3.9,
            description: "2 items",
            image: "https://fnb.dktcdn.net/media/100/071/941/products/abe73858-92d4-486b-9048-22c9bc28b8ca.jpg",
        },
        {
            name: "Combo for 2 people",
            price: 5.9,
            description: "3 items",
            image: "https://fnb.dktcdn.net/media/100/071/941/products/081bb524-72c1-411e-be70-0b5b5265321a.jpg",
        },
        {
            name: "Pangasius Hotpot",
            price: 7.9,
            description: "3 items",
            image: "https://fnb.dktcdn.net/media/100/071/941/products/90ec43b3-e56d-4632-84e3-fb648389d291.jpg",
        },
    ];

    const handleOpenModal = (item: any) => {
        setSelectedItem(item);
        setQuantity(1); // Reset quantity when opening modal
        setModalOpen(true);
    };

    const handleCloseModal = () => {
        setModalOpen(false);
        setSelectedItem(null);
    };

    const handleQuantityChange = (value: number) => {
        if (quantity + value >= 1) {
            setQuantity(quantity + value);
        }
    };

    const handleAddToCart = () => {
        const existingItem = cartItems.find(i => i.name === selectedItem.name);
        if (existingItem) {
            // Update the item if it already exists in the cart
            setCartItems(cartItems.map(i =>
                i.name === selectedItem.name
                    ? { ...i, quantity: i.quantity + quantity, note }
                    : i
            ));
        } else {
            // Add new item to cart
            setCartItems([...cartItems, { ...selectedItem, quantity, note }]);
        }
        setModalOpen(false);
        setSelectedItem(null);
        setQuantity(1);
        setNote("");
    };

    const totalPrice = cartItems.reduce((total, item) => total + item.price * item.quantity, 0);

    // Calculate total number of products in cart
    const totalProducts = cartItems.reduce((total, item) => total + item.quantity, 0);

    return (
        <div className="flex flex-col md:flex-row h-screen">
            {/* Sidebar */}
            <div className="bg-gray-100 p-4 md:w-64">
                <h2 className="text-xl font-bold mb-4">Menu</h2>
                <ul>
                    {categories.map((category) => (
                        <li
                            key={category.name}
                            className={`p-2 mb-2 rounded-lg cursor-pointer ${selectedCategory === category.name
                                ? "bg-blue-500 text-white"
                                : "hover:bg-gray-200"
                                }`}
                            onClick={() => setSelectedCategory(category.name)}
                        >
                            {category.name} ({category.count})
                        </li>
                    ))}
                </ul>
            </div>

            {/* Content */}
            <div className="flex-1 bg-white p-6">
                <div className="flex justify-between items-center mb-4">
                    <h1 className="text-2xl font-bold">{selectedCategory}</h1>
                    <button
                        className="bg-gray-300 text-gray-700 px-4 py-2 rounded-lg hover:bg-gray-400"
                        onClick={() => navigate('/')} // Navigate back to home page
                    >
                        Go Back
                    </button>
                </div>
                <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
                    {items.map((item, index) => (
                        <div
                            key={index}
                            className="flex flex-col items-center md:flex-row p-4 border rounded-lg shadow hover:shadow-lg"
                        >
                            <img
                                src={item.image}
                                alt={item.name}
                                className="w-16 h-16 rounded-lg mb-4 md:mb-0 md:mr-4"
                            />
                            <div className="text-center md:text-left">
                                <h3 className="text-lg font-bold">{item.name}</h3>
                                <p className="text-sm text-gray-500">{item.description}</p>
                                <p className="text-blue-600 font-bold">${item.price.toFixed(2)}</p>
                            </div>
                            <button
                                className="mt-4 md:mt-0 ml-auto bg-blue-500 text-white px-4 py-2 rounded-lg hover:bg-blue-600"
                                onClick={() => handleOpenModal(item)}
                            >
                                +
                            </button>
                        </div>
                    ))}
                </div>
            </div>

            {/* Modal */}
            {modalOpen && selectedItem && (
                <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
                    <div className="bg-white p-6 rounded-lg w-11/12 md:w-96 shadow-lg relative">
                        <button
                            className="absolute top-4 right-4 text-gray-500"
                            onClick={handleCloseModal}
                        >
                            ✕
                        </button>
                        <div className="flex items-center mb-4">
                            <img
                                src={selectedItem.image}
                                alt={selectedItem.name}
                                className="w-20 h-20 rounded-lg mr-4"
                            />
                            <div>
                                <h2 className="text-xl font-bold">{selectedItem.name}</h2>
                                <p className="text-sm text-gray-500">{selectedItem.description}</p>
                            </div>
                        </div>
                        <div className="mb-4">
                            <p className="mb-2">
                                <strong>Item Type:</strong>
                            </p>
                            <p className="text-blue-600 font-bold">Dine-in (${selectedItem.price.toFixed(2)})</p>
                        </div>
                        <div className="mb-4">
                            <textarea
                                className="w-full p-2 border rounded-lg"
                                placeholder="Enter notes to help the restaurant serve you better"
                                value={note}
                                onChange={(e) => setNote(e.target.value)}
                            ></textarea>
                        </div>
                        <div className="flex items-center justify-between">
                            <div className="flex items-center">
                                <button
                                    className="bg-gray-300 px-2 py-1 rounded-lg"
                                    onClick={() => handleQuantityChange(-1)}
                                >
                                    -
                                </button>
                                <span className="mx-4">{quantity}</span>
                                <button
                                    className="bg-blue-500 text-white px-2 py-1 rounded-lg"
                                    onClick={() => handleQuantityChange(1)}
                                >
                                    +
                                </button>
                            </div>
                            <p className="font-bold text-blue-600">
                                ${(selectedItem.price * quantity).toFixed(2)}
                            </p>
                        </div>
                        <button className="w-full mt-4 bg-blue-500 text-white py-2 rounded-lg" onClick={handleAddToCart}>
                            Add to Cart
                        </button>
                    </div>
                </div>
            )}

            {/* Conditionally render Cart Footer only if there are items in the cart */}
            {cartItems.length > 0 && (
                <div className="fixed bottom-0 left-0 right-0 bg-white p-4 shadow-lg flex justify-between items-center border-t">
                    <div className="flex items-center">
                        <span className="text-blue-500 mr-2">🛒 Total Products: {totalProducts}</span>
                        <span className="font-bold text-red-500 ml-4">Total Price: ${totalPrice.toLocaleString()}</span>
                    </div>
                    <button
                        className="bg-green-500 text-white px-6 py-2 rounded-lg"
                        onClick={() => navigate('/checkout')}
                    >
                        Order Now
                    </button>
                </div>
            )}
        </div>
    );
};

export default Order;
