import { createRouter, createWebHistory } from "vue-router"
import PhoneBook from "../components/PhoneBook.vue"

const routes = [
    {
        path: "/",
        name: "contacts",
        component: PhoneBook
    },
    {
        path: "/create",
        name: "create",
        component: () => import("../components/CreatePhoneBookItem.vue")
    }
]

const router = createRouter({
    history: createWebHistory(process.env.BASE_URL),
    routes
})

export default router
