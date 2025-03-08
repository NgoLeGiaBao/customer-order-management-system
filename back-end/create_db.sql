CREATE DATABASE db_mt_soa_authentication_service;
GO
CREATE DATABASE db_mt_soa_menu_service;
GO
CREATE DATABASE db_mt_soa_order_service;
GO
CREATE DATABASE db_mt_soa_payment_service;
GO
CREATE DATABASE db_mt_soa_welcome_service;
GO

-- Assign db_owner role to sa
ALTER ROLE db_owner ADD MEMBER sa;
GO
