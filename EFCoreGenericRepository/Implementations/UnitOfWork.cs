using EFCoreGenericRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EFCoreGenericRepository.Implementations
{
    /// <summary>
    /// ������Ԫ��Unit of Work������ʵ�֣��ۺϵ��� <typeparamref name="TDbContext"/> ������д�������
    /// ��������߽����ύʱ����������ҵ�������ɢ��Ķ�� <c>SaveChangesAsync()</c> ���á�
    /// </summary>
    /// <typeparam name="TDbContext">����� DbContext ���ͣ����� DI ��ע��Ϊ Scoped��</typeparam>
    public sealed class UnitOfWork<TDbContext> : IUnitOfWork<TDbContext>
        where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;
        private IDbContextTransaction _transaction;
        private Repository<TDbContext> _repository;
        private bool _disposed;

        /// <summary>
        /// ���칤����Ԫʵ����
        /// </summary>
        /// <param name="dbContext">������ע���ṩ�� DbContext ʵ����Scoped �������ڣ�</param>
        /// <exception cref="ArgumentNullException">�� <paramref name="dbContext"/> Ϊ��ʱ�׳�</exception>
        public UnitOfWork(TDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// �ù�����Ԫ�ڲ�ʹ�õĲִ�ʵ��
        /// </summary>
        /// <remarks>
        /// ʹ��ͬһ�� <see cref="DbContext"/>��
        /// ����ʱָ�� <c>autoSaveChanges:false</c>��д����ֻ�������״̬����������⣻
        /// ��ͨ�� <see cref="SaveChangesAsync"/> �� <see cref="CommitAsync"/> ���������־û���
        /// </remarks>
        public IRepository<TDbContext> Repository => _repository ??= new Repository<TDbContext>(_dbContext, autoSaveChanges: false);

        /// <summary>
        /// ��¶�ײ� DbContext��������Ҫ���� EF Core ���� / ԭ�� API ʱʹ�ã���������й©����ʾ�㣩
        /// </summary>
        public TDbContext DbContext => _dbContext;

        /// <summary>
        /// �Ƿ���ڻ�Ծ����
        /// </summary>
        public bool HasActiveTransaction => _transaction != null;

        /// <summary>
        /// ��ʽ�������ݿ��������Ѵ��ڻ��������Ա��ε��ã�
        /// </summary>
        /// <param name="cancellationToken">ȡ������</param>
        /// <remarks>
        /// ���飺����Ҫ��֤�����д��������ɹ���ȫ��ʧ�ܡ���ҵ���裬Ӧ��ʽ���á�
        /// δ��ʽ������ֱ�� Commit��ֻ�ܻ�õ��α����ԭ���ԣ�EF Core ��ʽ���񣩣��޷����Ƕ����ɢ��д�벽�衣
        /// </remarks>
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null) return;
            _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// ���浱ǰ DbContext ���ٵ����б�����������ύ/��������
        /// </summary>
        /// <param name="cancellationToken">ȡ������</param>
        /// <returns>��Ӱ�������</returns>
        /// <remarks>
        /// ������
        /// 1. ���������־û���ǰ��������Լƻ��Ժ������ͬһ�����в��������� <see cref="BeginTransactionAsync"/>����
        /// 2. ����Ҫ����ֻ�Ǽ��ύһ�Ρ�
        /// ע�⣺�ڻ�����е��ø÷���������������������ʽ���� <see cref="CommitAsync"/>��
        /// </remarks>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// �ύ��ǰ������δ��ʽ����������ִ��һ����ͨ����
        /// </summary>
        /// <param name="cancellationToken">ȡ������</param>
        /// <remarks>
        /// ��Ϊ��
        /// ������ִ�� <c>SaveChangesAsync</c> �� Commit�����ͷ��������
        /// �����񣺽�ִ��һ�� <c>SaveChangesAsync</c>��EF Core �ڲ��԰�����ʽ�����񣩡�
        /// ���飺��Ҫ��������ĳ�������ڵ���ǰʹ�� <see cref="BeginTransactionAsync"/>��
        /// </remarks>
        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                // ����ģʽ��δ��ʽ����ʱ������ֱ�ӱ��棨�ɸ����Ŷӹ淶��Ϊ���쳣�ϸ�ģʽ��
                await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return;
            }
            else
            {
                await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                await _transaction.DisposeAsync().ConfigureAwait(false);
                _transaction = null;
            }
        }

        /// <summary>
        /// �ع���ǰ�����������ʱ��Ĭ����
        /// </summary>
        /// <param name="cancellationToken">ȡ������</param>
        /// <remarks>
        /// �ع����ڴ��е��Ѹ���ʵ��״̬�����Զ���ԭΪ���ݿ�����ֵ���ɰ����ֶ�ˢ�»� Detach��
        /// </remarks>
        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null) return;
            await _transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            await _transaction.DisposeAsync().ConfigureAwait(false);
            _transaction = null;
        }

        /// <summary>
        /// �����װ�����Զ�ִ�С��������� �� ִ��ҵ��ί�� �� �ɹ��ύ / ʧ�ܻع�������
        /// </summary>
        /// <param name="action">����д������ί�У�ʹ���ṩ�Ĳִ�ʵ����</param>
        /// <param name="cancellationToken">ȡ������</param>
        /// <exception cref="ArgumentNullException">�� <paramref name="action"/> Ϊ��ʱ�׳�</exception>
        /// <remarks>
        /// Ƕ����Ϊ������ǰ���л�����򲻻����¿������񣬽����ø����񻷾���
        /// ʹ�ó�������Ӧ�÷����п��ٰ���һ��������ԭ�Ӳ���������ģ��������롣
        /// </remarks>
        public async Task ExecuteInTransactionAsync(
            Func<IRepository<TDbContext>, Task> action,
            CancellationToken cancellationToken = default)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            if (!HasActiveTransaction)
            {
                await BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    await action(Repository).ConfigureAwait(false);
                    await CommitAsync(cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    await RollbackAsync(cancellationToken).ConfigureAwait(false);
                    throw;
                }
            }
            else
            {
                // �����������񣨱���Ƕ���ظ�������
                await action(Repository).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// �ͷŹ�����Ԫ�ڲ�ռ�õķ��й�/�й���Դ����Ҫ�ǻ����
        /// </summary>
        /// <remarks>
        /// �����ͷ� <see cref="DbContext"/> ʵ���������������� DI Scoped ����
        /// ��ε��ð�ȫ���ݵȣ���
        /// </remarks>
        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;

            if (_transaction != null)
            {
                await _transaction.DisposeAsync().ConfigureAwait(false);
                _transaction = null;
            }
        }
    }
}