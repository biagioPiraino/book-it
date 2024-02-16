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
        // route level code-splitting
        // this generates a separate chunk (Home-[hash].js) for this route
        // which is lazy-loaded when the route is visited.
        component: () => import('@/views/HomePage.vue'),
      },
      {
        path: '/logout',
        name: 'logout',
        component: () => import('@/views/LogoutPage.vue'),
      },
      {
        path: '/desks',
        name: 'Desks',
        component: () => import('@/views/DesksPage.vue'),
      },
      {
        path:'/desks/create',
        name: 'Create a Desk',
        component: () => import('@/views/CreateDeskPage.vue'),
      },
      {
        path:'/desks/edit',
        name: 'Edit a Desk',
        props: {},
        component: () => import('@/views/EditDeskPage.vue'),
      }
    ],
  },
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,
})

export default router
