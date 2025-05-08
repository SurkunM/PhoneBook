<template>
    <v-dialog v-model="isShow" max-width="400px">
        <v-card>
            <v-card-title class="headline">
                Подтверждение удаления
            </v-card-title>
            <v-card-text>
                Вы действительно хотите удалить выбранный контакт?
            </v-card-text>
            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="error" text @click="$emit('delete')">Удалить</v-btn>
                <v-btn color="info" text @click="hide">Отмена</v-btn>
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

        emits: ["delete"]
    }
</script>