# Generated by Django 2.1.15 on 2021-02-18 09:36

from django.db import migrations, models


class Migration(migrations.Migration):

    initial = True

    dependencies = [
    ]

    operations = [
        migrations.CreateModel(
            name='Agents',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('id_customer', models.IntegerField()),
                ('os', models.IntegerField()),
                ('serial_number', models.IntegerField()),
                ('ip_public', models.IntegerField()),
                ('ip_private', models.IntegerField()),
                ('name_computer', models.IntegerField()),
                ('is_del', models.IntegerField()),
                ('time_create_at', models.IntegerField(default=1613641008)),
            ],
        ),
        migrations.CreateModel(
            name='backup_bytesave',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('id_agent', models.IntegerField()),
                ('id_metric_service', models.IntegerField()),
                ('name', models.CharField(blank=True, max_length=250)),
                ('local_path', models.CharField(blank=True, max_length=250)),
                ('container_name', models.CharField(blank=True, max_length=250)),
                ('email', models.CharField(blank=True, max_length=250)),
                ('time_delete', models.IntegerField()),
                ('max_concurrency', models.IntegerField()),
                ('is_folder', models.IntegerField()),
                ('time_last_run', models.IntegerField()),
                ('is_del', models.IntegerField()),
                ('time_create_at', models.IntegerField(default=1613641008)),
                ('time_update_at', models.IntegerField(default=1613641008)),
            ],
        ),
        migrations.CreateModel(
            name='connect_bytesave',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('id_agent', models.IntegerField()),
                ('id_metric_service', models.IntegerField()),
                ('name', models.CharField(blank=True, max_length=250)),
                ('type', models.IntegerField()),
                ('is_del', models.IntegerField()),
                ('time_create_at', models.IntegerField(default=1613641008)),
                ('time_update_at', models.IntegerField(default=1613641008)),
            ],
        ),
        migrations.CreateModel(
            name='Customer_Represent',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('id_customer', models.IntegerField()),
                ('name', models.CharField(blank=True, max_length=250)),
                ('phone_number', models.CharField(blank=True, max_length=250)),
                ('position', models.CharField(blank=True, max_length=250)),
                ('email', models.CharField(blank=True, max_length=250)),
                ('type', models.IntegerField(null=True)),
                ('is_del', models.IntegerField()),
                ('time_create_at', models.IntegerField(default=1613641008)),
                ('time_update_at', models.IntegerField(default=1613641008)),
            ],
        ),
        migrations.CreateModel(
            name='Customers',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('id_loggin', models.IntegerField(default=0)),
                ('id_version', models.IntegerField(default=1)),
                ('bytesave_email', models.CharField(max_length=250)),
                ('bytesave_pwd', models.CharField(max_length=250)),
                ('bytesave_amount_used', models.IntegerField(default=1)),
                ('bytesave_duration', models.IntegerField(blank=True, null=True)),
                ('bytesave_time_type', models.IntegerField(default=1)),
                ('bytesave_expiration_date', models.CharField(blank=True, max_length=250)),
                ('name', models.CharField(max_length=250)),
                ('email', models.CharField(max_length=250)),
                ('type', models.IntegerField(default=1)),
                ('phone_number', models.CharField(blank=True, max_length=250)),
                ('city', models.IntegerField(default=1)),
                ('address', models.CharField(blank=True, max_length=250)),
                ('tax_code', models.CharField(blank=True, max_length=250)),
                ('website', models.CharField(blank=True, max_length=250)),
                ('fax', models.CharField(blank=True, max_length=250)),
                ('legal_representative', models.CharField(blank=True, max_length=250)),
                ('scale', models.IntegerField(default=1)),
                ('field', models.IntegerField(default=1)),
                ('is_del', models.IntegerField()),
                ('time_create_at', models.IntegerField(default=1613641008)),
                ('time_update_at', models.IntegerField(default=1613641008)),
            ],
        ),
        migrations.CreateModel(
            name='Loggin',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('email', models.CharField(max_length=250)),
                ('name', models.CharField(max_length=250)),
                ('pwd', models.IntegerField(null=True)),
                ('type', models.IntegerField(null=True)),
                ('is_lock', models.IntegerField(default=0)),
                ('is_del', models.IntegerField()),
                ('time_create_at', models.IntegerField(default=1613641008)),
                ('time_update_at', models.IntegerField(default=1613641008)),
            ],
        ),
        migrations.CreateModel(
            name='Metric_Services',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('id_customer', models.IntegerField()),
                ('id_service', models.IntegerField()),
                ('information_connect', models.CharField(blank=True, max_length=250)),
                ('max_storage', models.IntegerField()),
                ('username_account', models.CharField(blank=True, max_length=250)),
                ('is_service', models.IntegerField()),
                ('status', models.IntegerField()),
                ('is_del', models.IntegerField()),
                ('time_create_at', models.IntegerField(default=1613641008)),
                ('time_update_at', models.IntegerField(default=1613641008)),
            ],
        ),
        migrations.CreateModel(
            name='Service',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('name', models.CharField(blank=True, max_length=100)),
                ('description', models.CharField(blank=True, max_length=250)),
                ('is_del', models.IntegerField()),
                ('time_create_at', models.IntegerField(default=1613641008)),
                ('time_update_at', models.IntegerField(default=1613641008)),
            ],
        ),
        migrations.CreateModel(
            name='Versions',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('name', models.CharField(blank=True, max_length=100)),
                ('description', models.CharField(blank=True, max_length=250)),
                ('is_del', models.IntegerField()),
                ('time_create_at', models.IntegerField(default=1613641008)),
                ('time_update_at', models.IntegerField(default=1613641008)),
            ],
        ),
    ]
