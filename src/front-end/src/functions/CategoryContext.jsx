import { createContext, useContext, useState, useEffect } from 'react';

const CategoryContext = createContext([]);

export function CategoryProvider({ children }) {
    const [categories, setCategories] = useState([]);

    useEffect(() => {
        // Фетчим категории один раз при монтировании компонента
        const fetchCategories = async () => {
            try {
                const response = await fetch(`http://localhost:5001/product/categories`);
                const data = await response.json();
                setCategories(data);
                //console.log(categories);
            } catch (error) {
                console.error("Error fetching categories:", error);
            }
        };

        fetchCategories();

    }, []); // Пустой массив зависимостей гарантирует, что запрос будет сделан только один раз

    return (
        <CategoryContext.Provider value={categories}>
            {children}
        </CategoryContext.Provider>
    );
}

export function useCategories() {
    return useContext(CategoryContext);
}