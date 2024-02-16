// Composables
import { createRouter, createWebHistory } from 'vue-router'

const routes = [
  {
    path: '/',
    component: () => import('@/layouts/MainLayout.vue'),
    children: [
      {
        path: '',
        name: 'Home',
        component: () => import('@/views/HomePage.vue'),
      },
      {
        path: '/search',
        name: 'Search',
        component: () => import('@/views/SearchPage.vue'),
      },
      {
        path: '/bookings',
        name: 'Bookings',
        component: () => import('@/views/BookingsPage.vue'),
      },
      {
        path: '/account',
        name: 'Account',
        component: () => import('@/views/AccountPage.vue'),
      },
    ],
  },
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,
})

export default router
