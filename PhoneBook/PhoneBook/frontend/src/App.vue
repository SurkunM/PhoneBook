<template>
    <v-card id="app">
        <v-layout>
            <v-app-bar color="grey">
                <v-app-bar-nav-icon @click="show = !show" size="x-large"></v-app-bar-nav-icon>
                <v-toolbar-title>Телефонная книга</v-toolbar-title>
            </v-app-bar>
            <v-navigation-drawer v-model="show"
                                 floating
                                 temporary
                                 color="grey-darken-3">
                <v-list density="compact" nav>
                    <v-list-item prepend-icon="mdi-account-multiple" to="/" title="Контакты"></v-list-item>
                    <v-list-item prepend-icon="mdi-account-plus" to="/create" title="Добавить"></v-list-item>
                </v-list>
            </v-navigation-drawer>

            <v-main>
                <router-view />
            </v-main>
        </v-layout>
    </v-card>
</template>

<script>
    export default {
        data() {
            return {
                show: false
            };
        }
    };
</script>