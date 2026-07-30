import 'package:flutter/material.dart';

import '../core/theme/app_theme.dart';

import '../features/auth/login_screen.dart';
import '../features/dashboard/dashboard_screen.dart';

import '../features/products/product_screen.dart';
import '../features/categories/category_screen.dart';
import '../features/purchase_orders/purchase_order_screen.dart';
import '../features/branches/branch_screen.dart';
import '../features/users/user_screen.dart';
import '../features/reports/report_screen.dart';
import '../features/settings/settings_screen.dart';

class App extends StatelessWidget {
  const App({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: "ALI'S Admin Portal",

      debugShowCheckedModeBanner: false,

      theme: AppTheme.light,

      initialRoute: "/",

      routes: {
        "/": (context) => const LoginScreen(),

        "/dashboard": (context) => const DashboardScreen(),

        "/products": (context) => const ProductScreen(),

        "/categories": (context) => const CategoryScreen(),

        "/purchase-orders": (context) =>
            const PurchaseOrderScreen(),

        "/branches": (context) => const BranchScreen(),

        "/users": (context) => const UserScreen(),

        "/reports": (context) => const ReportScreen(),

        "/settings": (context) => const SettingScreen(),
      },
    );
  }
}