import {useState} from "react";
import Path from "../../../endpoints.jsx";
import {useNavigate} from "react-router-dom";

const ProductRow = ({product, isNew}) => {
    const navigate = useNavigate();
    const [isEditing, setIsEditing] = useState(isNew);
    const [editedProduct, setEditedProduct] = useState({...product});

    const handleEditClick = () => {
        setIsEditing(true);
        setEditedProduct({...product});
    };

    const handleChange = (e) => {
        const {name, value} = e.target;
        setEditedProduct({...editedProduct, [name]: value});
    };

    const handleFileChange = (e) => {
        const file = e.target.files[0];
        if (file) {
            const maxSize = 5 * 1024 * 1024; // 5 MB
            if (file.size > maxSize) {
                alert("The file size is too large. Please select a file less than 5MB.");
                return; // Не сохраняем файл, если он слишком большой
            }
            setEditedProduct({...editedProduct, photo: file}); // сохраняем сам файл, а не Base64 строку
        }
    };

    const handleSaveClick = async () => {
        setIsEditing(false);
        try {
            const token = localStorage.getItem("authToken");
            //console.log(token);

            const method = isNew ? "POST" : "PUT"; // Если новый - POST, иначе PUT
            const url = isNew ? Path['createProduct'] : Path['product']; // Разные URL

            const formData = new FormData();
            formData.append("productId", editedProduct.id);
            formData.append("name", editedProduct.name);
            formData.append("description", editedProduct.description);
            formData.append("category", editedProduct.category);
            formData.append("price", editedProduct.price);
            formData.append("quantity", editedProduct.quantity);

            // Если есть изображение, добавляем его
            if (editedProduct.photo) {
                formData.append("image", editedProduct.photo);  // photo - это файл, загруженный пользователем
            }


            const response = await fetch(url, {
                method,
                headers: {
                    "Authorization": `Bearer ${token}`,
                },
                body: formData,  // отправляем FormData как тело запроса
            });

            if (!response.ok) {
                throw new Error("Failed to update/create product");
            }
            navigate(0);
            console.log("Product updated/created successfully");
        } catch (error) {
            console.error("Error updating/creating product:", error);
        }
    };

    const handleDeleteClick = async () => {
        try {
            const token = localStorage.getItem("authToken");

            // Проверяем, есть ли токен
            if (!token) {
                console.error("No authentication token found.");
                return;
            }

            const response = await fetch(Path['product'], {
                method: "DELETE",
                headers: {
                    "Authorization": `Bearer ${token}`,
                    "Content-Type": "application/json",  // Устанавливаем Content-Type для JSON
                },
                body: JSON.stringify({
                    productId: product.id,
                }),
            });

            if (!response.ok) {
                throw new Error("Failed to delete product");
            }
            navigate(0);
            console.log("Product deleted successfully");
        } catch (error) {
            console.error("Error deleting product:", error);
        }
    };

    return (
        <tr>
            <td>{product.id}</td>
            <td>
                {isEditing ? (
                    <input type="file" value="" className="input-group" style={{width: "100%"}} accept="image/*"
                           onChange={handleFileChange}/>
                ) : (
                    <img style={{maxWidth: "100px", objectFit: "contain"}} src={product.imageUrl || "#"} className="photo" alt="Photo"/>
                )}
            </td>
            <td>
                {isEditing ? (
                    <input type="text" className="input-group-text" style={{width: "100%"}} name="name"
                           value={editedProduct.name} onChange={handleChange}/>
                ) : (
                    product.name
                )}
            </td>
            <td>
                {isEditing ? (
                    <input type="text" className="input-group-text" style={{width: "100%"}} name="description"
                           value={editedProduct.description} onChange={handleChange}/>
                ) : (

                    <div style={{ maxHeight: "120px", overflowY: "auto" }}>
                        {product.description}
                    </div>
                )}
            </td>
            <td>
                {isEditing ? (
                    <input type="text" className="input-group-text" style={{width: "100%"}} name="category"
                           value={editedProduct.category} onChange={handleChange}/>
                ) : (
                    product.category
                )}
            </td>
            <td>
                {isEditing ? (
                    <input type="number" className="input-group-text" style={{width: "100%"}} name="price"
                           value={editedProduct.price} onChange={handleChange}/>
                ) : (
                    product.price
                )}
            </td>
            <td>
                {isEditing ? (
                    <input type="number" className="input-group-text" style={{width: "100%"}} name="quantity"
                           value={editedProduct.quantity} onChange={handleChange}/>
                ) : (
                    product.quantity
                )}
            </td>
            <td>{product.rating}/5</td>
            <td>{new Date(product.createdAt).toLocaleString()}</td>
            <td>{new Date(product.updatedAt).toLocaleString()}</td>
            <td>
                {isEditing ? (
                    <a className="save" title="Save" onClick={handleSaveClick}>
                        <i className="material-icons">&#xE161;</i>
                    </a>
                ) : (
                    <a className="edit" title="Edit" onClick={handleEditClick}>
                        <i className="material-icons">&#xE254;</i>
                    </a>
                )}
                <a className="delete" title="Delete" onClick={handleDeleteClick}>
                    <i className="material-icons">&#xE872;</i>
                </a>
            </td>
        </tr>
    );
};

export default ProductRow;