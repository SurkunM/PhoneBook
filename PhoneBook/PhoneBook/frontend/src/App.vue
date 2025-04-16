<template>
    <v-card>
        <v-layout>
            <v-navigation-drawer floating
                                 permanent
                                 color="grey-darken-3">
                <v-list density="compact" nav>
                    <h1 class="mb-4">Телефонная книга</h1>
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

<!--<script>
    export default {
        name: "App",

        components: {

        }
    };
</script>-->
