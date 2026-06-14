# Liora Frontend Replacement Plan

We are in the process of replacing the old frontend with the new premium design provided by the user. Below is the updated status of the frontend views.

## 1. Customer Area (Storefront)

### Completed
- Products/Index (Products Listing / Filter)
- Products/Details (Product Details, Image Gallery, Add to Cart)
- Cart/Index (Cart Page)
- Cart/Checkout (Checkout Form and Payment)
- Orders/Index (My Orders List)
- Orders/Success (Order Placed Success Page)

### Missing / Needs UI Templates
- **Orders/Details**: The template you provided for Orders/Details actually contained the Orders/Index (My Orders) markup. We need the real **Order Details** UI template (showing specific items inside a single order).
- **Home/Index**: The main Landing/Home page template.
- **Home/Privacy / Home/About**: Any static pages if they exist.

## 2. Identity Area (Authentication & Profile)

### Missing / Needs UI Templates (Authentication)
- **Account/Login**: The login page template.
- **Account/Register**: The registration page template.
- **Account/ForgotPassword** & **Account/ResetPassword**: Password recovery templates.
- **Account/AccessDenied**: Access denied page template.

### Missing / Needs UI Templates (Profile / Dashboard)
- **Profile/Index**: The user's main account dashboard/profile details.
- **Profile/Addresses** & **Profile/AddAddress** & **Profile/EditAddress**: Address book management.
- **Profile/ChangePassword** & **Profile/ChangeEmail**: Account settings pages.

## 3. Admin Area (Dashboard)

### Missing / Needs UI Templates
*None of the Admin pages have been replaced yet. The user needs to provide the new UI templates for the Admin dashboard.*
- **Dashboard Home** (Admin/Home/Index)
- **Products Management** (Admin/Products/Index, Create, Edit)
- **Categories Management** (Admin/Categories/Index, Create, Edit)
- **Orders Management** (Admin/Orders/Index, Details)
- **Users Management** (Admin/Users/Index)
- **Gift Bundles & Discounts** (Admin/GiftBundles/Index, Admin/Discounts/Index)

