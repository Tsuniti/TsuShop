const Path = {};
console.log(import.meta.env.VITE_API_HOST);
Path['host'] = import.meta.env.VITE_API_HOST;
Path['auth'] = Path['host']+'/auth';
Path['login'] = Path['auth'] + "/login";
Path['register'] = Path['auth']+'/register';
Path['product'] = Path['host']+'/product';
Path['productsMaxPrice'] = Path['product']+'/max-price';
Path['productCategories'] = Path['product']+'/categories';
Path['createProduct'] = Path['product']+'/create';
Path['removeFromStockProduct'] = Path['product']+'/remove-from-stock';
Path['addToStockProduct'] = Path['product']+'/add-to-stock';
Path['review'] = Path['host']+'/review';
Path['email'] = Path['host']+'/email';
Path['cart'] = Path['host']+'/cart';

export default Path;