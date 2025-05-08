<template>
    <v-dialog v-model="isShow" max-width="400px">
        <v-card>
            <v-card-title class="headline">
                Подтверждение скачивания
            </v-card-title>
            <v-card-text>
                Скачать все контакты в excel файл?
            </v-card-text>
            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="info" text @click="$emit('export')">Скачать</v-btn>
                <v-btn color="error" text @click="hide">Отмена</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script>
    export default {
        data() {
            return {
                isShow: false
            };
        },

        methods: {
            show() {
                this.isShow = true;
            },

            hide() {
                this.isShow = false;
            }
        },

        emits: ["export"]
    }
</script>