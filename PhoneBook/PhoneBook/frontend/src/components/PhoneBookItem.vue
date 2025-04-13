<template>
    <tr>
        <!-- Чекбокс -->
        <td>
            <v-simple-checkbox  />
        </td>

        <td>{{ index + 1 }}</td>

        <!-- Фамилия -->
        <td>{{ contact.lastName }}</td>

        <!-- Имя -->
        <td>{{ contact.firstName }}</td>

        <td>{{ contact.phone }}</td>


        <!-- Кнопки действий -->
        <td>
            <v-btn small
                   color="error">
                <!--@click="$emit('delete', contact)"@click="$emit('edit', contact)"-->
                Удалить
            </v-btn>
            <v-btn small
                   color="primary"
                   class="ml-2">
                Редактировать
            </v-btn>
        </td>
    </tr>
</template>

<script>
    export default {
        props: {
            contact: {
                type: Object,
                required: true
            },
            index: {
                type: Number,
                required: true
            }
        }
    }
</script>